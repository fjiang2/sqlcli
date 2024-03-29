﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using Sys;
using Sys.CodeBuilder;
using Sys.Data;
using Sys.Data.Entity;
using Sys.Stdio;

namespace Sys.Data.Code
{

    public class DataContract2ClassBuilder : DataTableClassBuilder
    {


        public DataContract2ClassBuilder(IApplicationCommand cmd, TableName tname, DataTable dt, bool allowDbNull)
            : base(cmd, tname, dt, allowDbNull)
        {

            builder.AddUsing("System");
            builder.AddUsing("System.Collections.Generic");
            builder.AddUsing("System.Data");
            builder.AddUsing("System.Linq");
            builder.AddUsing("Sys.Data");

            AddOptionalUsing();
        }




        protected override void CreateClass()
        {
            TypeInfo[] _base = new TypeInfo[]
            {
                //new TypeInfo { Type = typeof(IDataContractRow) },
                new TypeInfo { Type = typeof(IEntityRow) },
                new TypeInfo { UserType = $"IEquatable<{ClassName}>" }
            };
            var clss = new Class(ClassName, _base)
            {
                Modifier = Modifier.Public | Modifier.Partial
            };
            builder.AddClass(clss);


            foreach (DataColumn column in dt.Columns)
            {
                clss.Add(new Property(dict[column], PropertyName(column)) { Modifier = Modifier.Public });
            }

            Constructor_Default(clss);

            if (ContainsMethod("FillObject"))
            {
                Constructor_DataRow(clss);
                Method_FillObject(clss);
            }
            if (ContainsMethod("UpdateRow"))
                Method_UpdateRow(clss);
            if (ContainsMethod("CopyTo"))
                Method_CopyTo(clss);
            if (ContainsMethod("Equals"))
                Method_Equals(clss);
            if (ContainsMethod("CreateTable"))
                Method_CreateTable(clss);
            if (ContainsMethod("ToDictionary"))
                Method_ToDictionary(clss);
            if (ContainsMethod("FromDictionary"))
                Constructor_FromDictionary(clss);

            //Method_CRUD(dt, clss);
            int index2 = clss.Index;

            if (ContainsMethod("ToString"))
                Method_ToString(clss);

            CreateTableSchemaFields(dt, clss);
            int index1 = clss.Index;
            clss.AppendLine();

            //Const Field
            foreach (DataColumn column in dt.Columns)
            {
                Field field = new Field(new TypeInfo { Type = typeof(string) }, COLUMN(column), new Value(column.ColumnName))
                {
                    Modifier = Modifier.Public | Modifier.Const
                };
                clss.Add(field);
            }

            var clssAssoc = Class_Assoication(clss, index1, index2);
            if (clssAssoc.Index > 0)
                builder.AddClass(clssAssoc);

        }

        private static void Constructor_Default(Class clss)
        {
            Constructor constructor = new Constructor(clss.Name)
            {
                Modifier = Modifier.Public,
            };

            clss.Add(constructor);
        }

        private static void Constructor_DataRow(Class clss)
        {
            Constructor constructor = new Constructor(clss.Name)
            {
                Modifier = Modifier.Public,
                Params = new Parameters().Add(typeof(DataRow), "row")
            };

            clss.Add(constructor);
            var sent = constructor.Body;
            sent.AppendLine("FillObject(row);");
        }

        private void Method_FillObject(Class clss)
        {
            Method mtdFillObject = new Method("FillObject")
            {
                Modifier = Modifier.Public,
                Params = new Parameters().Add(typeof(DataRow), "row")
            };
            clss.Add(mtdFillObject);
            var sent = mtdFillObject.Body;

            foreach (DataColumn column in dt.Columns)
            {
                var type = dict[column];
                var NAME = COLUMN(column);
                var name = PropertyName(column);

                var line = $"this.{name} = row.{GetField}<{type}>({NAME});";
                sent.AppendLine(line);
            }
        }

        private void Method_UpdateRow(Class clss)
        {
            Method mtdUpdateRow = new Method("UpdateRow")
            {
                Modifier = Modifier.Public,
                Params = new Parameters().Add(typeof(DataRow), "row")
            };
            clss.Add(mtdUpdateRow);
            var sent = mtdUpdateRow.Body;
            foreach (DataColumn column in dt.Columns)
            {
                var NAME = COLUMN(column);
                var name = PropertyName(column);

                var line = $"row.SetField({NAME}, this.{name});";
                sent.AppendLine(line);
            }
        }

        private void Method_CopyTo(Class clss)
        {
            Method mtdCopyTo = new Method("CopyTo")
            {
                Modifier = Modifier.Public,
                Params = new Parameters().Add(ClassName, "obj")
            };
            clss.Add(mtdCopyTo);
            var sent = mtdCopyTo.Body;

            foreach (DataColumn column in dt.Columns)
            {
                var name = PropertyName(column);

                var line = $"obj.{name} = this.{name};";
                sent.AppendLine(line);
            }
        }

        private void Method_Equals(Class clss)
        {
            Method mtdEquals = new Method("Equals")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo { Type = typeof(bool) },
                Params = new Parameters().Add(ClassName, "obj")
            };
            clss.Add(mtdEquals);
            Statement sent = mtdEquals.Body;
            sent.AppendLine("return ");
            IEnumerable<string> variables = dict.Keys.Select(column => PropertyName(column));
            variables.ForEach(
                variable => sent.Append($"this.{variable} == obj.{variable}"),
                variable => sent.AppendLine("&& ")
            );

            sent.Append(";");
        }

        private void Method_ToDictionary(Class clss)
        {
            Method method = new Method("ToDictionary")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo { Type = typeof(IDictionary<string, object>) },
            };
            clss.Add(method);
            Statement sent = method.Body;
            sent.AppendLine("return new Dictionary<string,object>() ");
            sent.Begin();
            int count = dt.Columns.Count;
            int i = 0;
            foreach (DataColumn column in dt.Columns)
            {
                Type ty = dict[column].Type;
                var name = COLUMN(column);
                var line = $"[{name}] = this.{PropertyName(column)}";
                if (++i < count)
                    line += ",";

                sent.AppendLine(line);
            }
            sent.End(";");
        }

        private void Constructor_FromDictionary(Class clss)
        {
            Constructor method = new Constructor(clss.Name)
            {
                Modifier = Modifier.Public,
                Params = new Parameters().Add(typeof(IDictionary<string, object>), "dict"),
            };
            clss.Add(method);
            Statement sent = method.Body;
            foreach (DataColumn column in dt.Columns)
            {
                var type = dict[column];
                var name = COLUMN(column);
                var line = $"this.{PropertyName(column)} = ({type})dict[{name}];";
                sent.AppendLine(line);
            }
        }

        private void Method_ToString(Class clss)
        {
            Method method = new Method(new TypeInfo { Type = typeof(string) }, "ToString")
            {
                Modifier = Modifier.Public | Modifier.Override
            };
            clss.Add(method);
            Statement sent = method.Body;

            IEnumerable<string> variables = dict.Keys.Select(column => PropertyName(column));
            StringBuilder sb = new StringBuilder("\"{{");
            int index = 0;
            variables.ForEach(
                variable => sb.Append($"{variable}:{{{index++}}}"),
                variable => sb.Append(", ")
                );

            sb.AppendLine("}}\", ");

            variables.ForEach(
                variable => sb.Append($"{variable}"),
                variable => sb.AppendLine(", ")
                );

            sent.AppendFormat("return string.Format({0});", sb);
            clss.AppendLine();

        }

        public static void Method_CRUD(DataTable dt, Class clss)
        {
            var provider = ConnectionProviderManager.DefaultProvider;
            TableName tname = new TableName(provider, dt.TableName);

            SqlGenerator gen = new SqlGenerator(tname.FormalName)
            {
                PrimaryKeys = dt.PrimaryKey.Select(x => x.ColumnName).ToArray()
            };

            foreach (DataColumn column in dt.Columns)
            {
                string cname = column.ColumnName;
                gen.Add(cname, "{" + cname + "}");
            }

            Method method = new Method("Insert")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo(typeof(string)),
            };
            method.Body.AppendLine("return $\"" + gen.Insert() + "\";");
            clss.Add(method);

            method = new Method("Update")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo(typeof(string)),
            };
            method.Body.AppendLine("return $\"" + gen.Update() + "\";");
            clss.Add(method);

            method = new Method("InsertOrUpdate")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo(typeof(string)),
            };
            method.Body.AppendLine("return $\"" + gen.InsertOrUpdate() + "\";");
            clss.Add(method);

            method = new Method("Delete")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo(typeof(string)),
            };
            method.Body.AppendLine("return $\"" + gen.Delete() + "\";");
            clss.Add(method);
        }


        private Class Class_Assoication(Class clss, int index1, int index2)
        {
            Class clssAssoc = new Class(ClassName + ASSOCIATION) { Modifier = Modifier.Public };

            bool hasFK = cmd.Has("fk");
            bool hasAssoc = cmd.Has("assoc");
            if (hasAssoc)
                hasFK = true;

            if (hasFK)
            {
                var field = CreateConstraintField(tname, string.Empty);
                if (field != null)
                    clss.Insert(index1, field);
            }

            if (hasAssoc)
            {
                var properties = CreateAssoicationClass(tname, clssAssoc);
                Method_Association(clss, index2, properties);
            }

            return clssAssoc;
        }

        private Method Method_Association(Class clss, int index, List<AssociationPropertyInfo> properties)
        {
            string associationClassName = ClassName + ASSOCIATION;
            Method method = new Method("GetAssociation")
            {
                Modifier = Modifier.Public,
                Type = new TypeInfo { UserType = associationClassName },
                Params = new Parameters().Add("IQuery", "query"),
            };
            Statement sent = method.Body;

            sent.Return($"GetAssociation(query, new {ClassName}[] {{ this }}).FirstOrDefault()");
            clss.Insert(index++, method);


            method = new Method("GetAssociation")
            {
                Modifier = Modifier.Public | Modifier.Static,
                Type = new TypeInfo { UserType = $"IEnumerable<{associationClassName}>" },
                Params = new Parameters().Add("IQuery", "query").Add($"IEnumerable<{ClassName}>", "entities"),
            };
            clss.Insert(index++, method);

            sent = method.Body;
            sent.AppendLine("var reader = query.Expand(entities);");
            sent.AppendLine();
            sent.AppendLine($"var associations = new List<{associationClassName}>();");
            sent.AppendLine();

            foreach (var property in properties)
            {
                sent.AppendLine($"var _{property.PropertyName} = reader.Read<{property.PropertyType}>();");
            }
            sent.AppendLine();

            sent.AppendLine("foreach (var entity in entities)");
            sent.Begin();
            sent.AppendLine($"var association = new {associationClassName}");
            sent.Begin();

            foreach (var p in properties)
            {
                if (p.OneToMany)
                    sent.AppendLine($"{p.PropertyName} = new EntitySet<{p.PropertyType}>(_{p.PropertyName}.Where(row => row.{p.FK_Column} == entity.{p.PK_Column})),");
                else
                    sent.AppendLine($"{p.PropertyName} = new EntityRef<{p.PropertyType}>(_{p.PropertyName}.FirstOrDefault(row => row.{p.FK_Column} == entity.{p.PK_Column})),");
            }

            sent.End(";");
            sent.AppendLine("associations.Add(association);");
            sent.End();

            sent.AppendLine();
            sent.AppendLine($"return associations;");
            return method;
        }
    }
}
