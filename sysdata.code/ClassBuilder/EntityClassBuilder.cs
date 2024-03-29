﻿using Sys;
using Sys.CodeBuilder;
using Sys.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Sys.Data.Manager;
using Sys.Stdio;

namespace Sys.Data.Code
{

    public class EntityClassBuilder : TheClassBuilder
    {
        private readonly TableName tname;
        public bool IsAssocication { get; private set; }

        public EntityClassBuilder(IApplicationCommand cmd, TableName tname)
            : base(cmd)
        {
            this.tname = tname;
            this.SetClassName(tname.ToClassName(rule: null));

            builder.AddUsing("System");
            builder.AddUsing("System.Collections.Generic");
            builder.AddUsing("System.Data");
            builder.AddUsing("System.Linq");

            AddOptionalUsing();
            IsAssocication = Associate();
        }

        /// <summary>
        /// check it is associative table
        /// </summary>
        /// <returns></returns>
        private bool Associate()
        {
            TableSchema schema = new TableSchema(tname);
            if (schema.Columns.Count != 2)
                return false;

            IColumn c1 = schema.Columns[0];
            IColumn c2 = schema.Columns[1];

            if (!c1.IsPrimary || !c2.IsPrimary)
                return false;

            var fk = schema.ForeignKeys;
            return fk.Length == 2;
        }

        protected override void CreateClass()
        {
            if (IsAssocication)
                return;

            TableSchema schema = new TableSchema(tname);
            static string COLUMN(IColumn column) => "_" + column.ColumnName.ToUpper();

            TypeInfo[] baseClass = OptionalBaseType();

            var clss = new Class(ClassName, OptionalBaseType())
            {
                Modifier = Modifier.Public | Modifier.Partial
            };

            builder.AddClass(clss);

            string optionField = cmd.GetValue("field");
            if (optionField != null)
            {
                string[] fields = optionField.Split(',');

                if (fields.Contains("const"))
                {
                    //Const Field
                    Field field;
                    foreach (var column in schema.Columns)
                    {
                        field = new Field(new TypeInfo { Type = typeof(string) }, COLUMN(column), new Value(column.ColumnName))
                        {
                            Modifier = Modifier.Public | Modifier.Const
                        };
                        clss.Add(field);
                    }
                }
            }


            UtilsThisMethod option = UtilsThisMethod.Undefined;

            if (ContainsMethod("Map"))
            {
                string identityColumn = schema.Columns
                    .Where(column => column.IsIdentity)
                    .Select(column => column.ColumnName)
                    .FirstOrDefault();

                if (identityColumn == null && schema.Columns[0].CType == CType.Int)
                    identityColumn = schema.Columns[0].ColumnName;

                if (identityColumn != null)
                {
                    const string IdentityName = "Identity";
                    Property identity = new Property(new TypeInfo(typeof(int)), IdentityName)
                    {
                        IsLambda = true,
                    };

                    var attr = base.Attributes;
                    if (attr.ContainsKey(IdentityName))
                    {
                        foreach (string x in attr[IdentityName])
                            identity.AddAttribute(new AttributeInfo(x));
                    }

                    identity.Gets.Append($"this.{identityColumn};");
                    clss.Add(identity);
                }

                //identity column excluded
                PropertyInfo[] columns = schema.Columns
                    .Where(column => !column.IsIdentity)
                    .Select(column => new PropertyInfo { PropertyName = column.ColumnName })
                    .ToArray();

                clss.AddUtilsMethod(ClassName, columns, UtilsThisMethod.Map);
            }

            if (ContainsMethod("Copy"))
                option |= UtilsThisMethod.Copy;

            if (ContainsMethod("Clone"))
                option |= UtilsThisMethod.Clone;

            if (ContainsMethod("Equals"))
                option |= UtilsThisMethod.Equals;

            if (ContainsMethod("GetHashCode"))
                option |= UtilsThisMethod.GetHashCode;

            if (ContainsMethod("Compare"))
                option |= UtilsThisMethod.Compare;

            if (ContainsMethod("ToDictionary"))
                option |= UtilsThisMethod.ToDictionary;

            if (ContainsMethod("ToString"))
                option |= UtilsThisMethod.ToString;

            {
                PropertyInfo[] columns = schema.Columns
                    .Select(column => new PropertyInfo
                    {
                        PropertyType = column.GetTypeInfo(),
                        PropertyName = column.ColumnName
                    })
                    .ToArray();

                clss.AddUtilsMethod(ClassName, columns, option);
            }
        }
    }
}
