﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Sys;
using Sys.CodeBuilder;
using Sys.Data;
using Sys.Data.Manager;
using Sys.Stdio;

namespace Sys.Data.Code
{
    public class Linq2SQLClassBuilder : TheClassBuilder
    {
        private readonly TableName tname;

        public Linq2SQLClassBuilder(IApplicationCommand cmd, TableName tname)
            : base(cmd)
        {
            this.tname = tname;
            this.SetClassName(tname.ToClassName(rule: null));

            builder.AddUsing("System");
            //builder.AddUsing("System.Collections.Generic");
            //builder.AddUsing("System.Data");
            builder.AddUsing("System.Data.Linq");
            builder.AddUsing("System.Data.Linq.Mapping");
        }


        protected override void CreateClass()
        {

            var clss = new Class(ClassName)
            {
                Modifier = Modifier.Public | Modifier.Partial
            };
            clss.AddAttribute(new AttributeInfo("Table", new { Name = tname.ShortName }));

            builder.AddClass(clss);

            CodeStyle camel = cmd.GetEnum<CodeStyle>("code-style", CodeStyle.Original);
            TableSchema schema = TableSchemaCache.GetSchema(tname);

            Property prop;
            foreach (IColumn column in schema.Columns)
            {
                TypeInfo ty = new TypeInfo { UserType = column.DataType.GetFieldType(column.Nullable) };

                prop = new Property(ty, column.ToFieldName(camel)) { Modifier = Modifier.Public };

                List<object> args = new List<object>();
                args.Add(new { Name = column.ColumnName });

                //args.Add(new { DbType = ColumnSchema.GetSQLType(column) + (column.Nullable ? " NULL" : " NOT NULL") });

                if (column.IsPrimary)
                    args.Add(new { IsPrimaryKey = true });

                if (column.IsIdentity)
                    args.Add(new { IsDbGenerated = true });

                if (!column.IsPrimary && !column.Nullable)
                    args.Add(new { CanBeNull = false });

                if (column.CType == CType.Text || column.CType == CType.NText)
                    args.Add(new AttributeInfoArg("UpdateCheck", "UpdateCheck.Never"));

                prop.AddAttribute(new AttributeInfo("Column", args.ToArray()));

                if (!column.IsComputed)
                    clss.Add(prop);

            }

            var fkBy = schema.ByForeignKeys.Keys.OrderBy(k => k.FK_Table);

            Constructor constructor = null;
            if (fkBy.Any())
            {
                clss.AppendLine();

                constructor = new Constructor(this.ClassName);
            }



            List<Property> list = new List<Property>();
            foreach (var key in fkBy)
            {
                prop = AddEntitySet(clss, constructor, key);
                list.Add(prop);
            }

            var fks = schema.ForeignKeys;
            //list = new List<Property>();

            if (fks.Length > 0)
                clss.AppendLine();

            foreach (var key in fks.Keys)
            {
                prop = AddEntityRef(clss, key);
                list.Add(prop);
            }

            if (constructor != null)
                clss.Add(constructor);

            foreach (var p in list)
                clss.Add(p);

        }



        /// <summary>
        /// add children tables
        /// </summary>
        /// <param name="clss"></param>
        /// <param name="constructor"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private Property AddEntitySet(Class clss, Constructor constructor, IForeignKey key)
        {
            TableName fk_tname = new TableName(tname.DatabaseName, key.FK_Schema, key.FK_Table);
            string fk_cname = fk_tname.ToClassName(rule: null);
            string pname;

            Property prop;
            TypeInfo ty;
            Field field;

            var fk_schema = TableSchemaCache.GetSchema(fk_tname);
            var _keys = fk_schema.PrimaryKeys.Keys;

            if (_keys.Length == 1 && _keys.Contains(key.FK_Column))
            {
                // 1:1 mapping
                pname = clss.MakeUniqueName(Plural.Singularize(fk_cname));
                ty = new TypeInfo { UserType = $"EntityRef<{fk_cname}>" };
                field = new Field(ty, $"_{pname}") { Modifier = Modifier.Private };

                prop = new Property(new TypeInfo { UserType = fk_cname }, pname) { Modifier = Modifier.Public };
                prop.Gets.Append($"return this._{pname}.Entity;");
                prop.Sets.Append($"this._{pname}.Entity = value;");

                prop.AddAttribute(new AttributeInfo("Association",
                 new
                 {
                     Name = $"{this.ClassName}_{fk_cname}",
                     Storage = $"_{pname}",
                     ThisKey = key.PK_Column,
                     OtherKey = key.FK_Column,
                     IsUnique = true,
                     IsForeignKey = false
                 }));
            }
            else
            {
                //1:n mapping
                pname = clss.MakeUniqueName(Plural.Pluralize(fk_cname));
                constructor.Body.AppendLine($"this._{pname} = new EntitySet<{fk_cname}>();");

                ty = new TypeInfo { UserType = $"EntitySet<{fk_cname}>" };
                field = new Field(ty, $"_{pname}") { Modifier = Modifier.Private };

                prop = new Property(ty, pname) { Modifier = Modifier.Public };
                prop.Gets.Append($"return this._{pname};");
                prop.Sets.Append($"this._{pname}.Assign(value);");

                prop.AddAttribute(new AttributeInfo("Association",
                 new
                 {
                     Name = $"{this.ClassName}_{fk_cname}",
                     Storage = $"_{pname}",
                     ThisKey = key.PK_Column,
                     OtherKey = key.FK_Column,
                     IsForeignKey = false
                 }));
            }

            clss.Add(field);


            return prop;
        }


        /// <summary>
        /// add foreighn keys
        /// </summary>
        /// <param name="clss"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private Property AddEntityRef(Class clss, IForeignKey key)
        {
            string pk_cname = new TableName(tname.DatabaseName, key.PK_Schema, key.PK_Table).ToClassName(rule: null);
            string pname = clss.MakeUniqueName(pk_cname);

            var field = new Field(new TypeInfo { UserType = $"EntityRef<{pk_cname}>" }, $"_{pname}") { Modifier = Modifier.Private };
            clss.Add(field);

            var prop = new Property(new TypeInfo { UserType = pk_cname }, pname) { Modifier = Modifier.Public };
            prop.Gets.Append($"return this._{pname}.Entity;");
            prop.Sets.Append($"this._{pname}.Entity = value;");
            prop.AddAttribute(new AttributeInfo("Association",
                new
                {
                    Name = $"{pk_cname}_{this.ClassName}",
                    Storage = $"_{pname}",
                    ThisKey = key.FK_Column,
                    OtherKey = key.PK_Column,
                    IsForeignKey = true
                }));
            return prop;
        }


    }
}
