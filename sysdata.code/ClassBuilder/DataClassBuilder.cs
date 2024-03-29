﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.CodeBuilder;
using Sys.Data;
using Sys.Data.Manager;
using Sys.Stdio;

namespace Sys.Data.Code
{

    public class DataClassBuilder : ClassMaker
    {
        private readonly DataTable dt;

        public DataClassBuilder(IApplicationCommand cmd, DataTable dt)
            : base(cmd)
        {
            this.cmd = cmd;
            this.dt = dt;
        }

        public void ExportCSharpData()
        {
            switch (dataType)
            {
                case DataClassType.Array:
                case DataClassType.List:
                case DataClassType.Dictionary:
                    ExportCSData(dt);
                    return;

                case DataClassType.Enum:
                    ExportEnum(dt);
                    return;

                case DataClassType.Constant:
                    ExportConstant(dt);
                    return;
            }
        }

        private DataClassType dataType
        {
            get
            {
                string _dataType = cmd.GetValue("type") ?? "list";
                return _dataType switch
                {
                    "array" => DataClassType.Array,
                    "list" => DataClassType.List,
                    "dict" => DataClassType.Dictionary,
                    "enum" => DataClassType.Enum,
                    "const" => DataClassType.Constant,
                    _ => DataClassType.Undefined,
                };
            }
        }




        protected override string ClassName
        {
            get
            {
                string tableName = dt.TableName;
                string _cname = base.ClassName;
                if (_cname != nameof(DataTable))
                    return _cname;

                if (!string.IsNullOrEmpty(tableName))
                {
                    //use table name as class name
                    string name = new string(tableName.Trim().Where(ch => char.IsLetterOrDigit(ch) || ch == '_').ToArray());
                    if (name.Length > 0 && char.IsDigit(name[0]))
                        name = $"_{name}";

                    if (name != string.Empty)
                        _cname = name;
                }

                return _cname;
            }
        }

        /// <summary>
        /// command: export /c# /code-column:Col1=Dictionary<int,string>;Col2=Dictionary<int,string>
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, TypeInfo> CodeColumnDef()
        {
            Dictionary<string, TypeInfo> dict = new Dictionary<string, TypeInfo>();
            var columns = cmd.GetValue("code-column");
            if (columns == null)
                return dict;
            string[] _columns = columns.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var column in _columns)
            {
                string[] kvp = column.Split('=');
                TypeInfo ty = new TypeInfo { UserType = kvp[1] };
                dict.Add(kvp[0], ty);
            }

            return dict;
        }


        /// <summary>
        /// create C# data from data table
        /// </summary>
        /// <param name="cmd"></param>
        public void ExportCSData(DataTable dt)
        {

            string dataclass = cmd.GetValue("dataclass") ?? "DbReadOnly";

            CSharpBuilder builder = new CSharpBuilder
            {
                Namespace = NamespaceName
            };

            builder.AddUsingRange(base.Usings);

            string cname = ClassName;

            Dictionary<string, TypeInfo> codeColumns = CodeColumnDef();
            var clss = new Class(cname)
            {
                Modifier = Modifier.Public | Modifier.Partial
            };

            if (!cmd.Has("dataonly"))
                builder.AddClass(clss);

            Property prop;
            foreach (DataColumn column in dt.Columns)
            {
                bool nullable = dt.AsEnumerable().Any(row => row[column] is DBNull);
                TypeInfo ty = new TypeInfo(column.DataType) { Nullable = nullable };
                if (codeColumns.ContainsKey(column.ColumnName))
                    ty = codeColumns[column.ColumnName];

                prop = new Property(ty, column.ColumnName.ToFieldName()) { Modifier = Modifier.Public };
                clss.Add(prop);
            }

            clss = new Class(dataclass)
            {
                Modifier = Modifier.Public | Modifier.Partial
            };

            if (!cmd.Has("classonly"))
                builder.AddClass(clss);


            string[] columns = dt.Columns.Cast<DataColumn>().Select(col => col.ColumnName).ToArray();

            string fieldName = cmd.GetValue("dataname") ?? $"{cname}Data";

            if (dataType == DataClassType.List || dataType == DataClassType.Array)
            {
                Field field = CreateListOrArrayField(fieldName, dataType, dt, cname, columns, codeColumns);
                clss.Add(field);
            }
            else
            {
                if (dt.Columns.Count < 2)
                {
                    cerr.WriteLine("cannot generate dictionary class, column# > 2");
                    return;
                }

                Field field = CreateDictionaryField(fieldName, dt, cname, columns, codeColumns);
                clss.Add(field);
            }

            PrintOutput(builder, cname);
        }

        private static Field CreateDictionaryField(string fieldName, DataTable dt, string cname, string[] columns, IDictionary<string, TypeInfo> codeColumns)
        {

            List<KeyValuePair<object, object>> L = new List<KeyValuePair<object, object>>();
            var keyType = new TypeInfo(dt.Columns[0].DataType);
            var valueType = new TypeInfo(dt.Columns[1].DataType);
            if (dt.Columns.Count != 2)
                valueType = new TypeInfo { UserType = cname };

            TypeInfo type = new TypeInfo { UserType = $"{cname}" };
            foreach (DataRow row in dt.Rows)
            {
                string key = Primitive.ToPrimitive(row[0]);

                if (dt.Columns.Count != 2)
                {
                    var instance = new New(type) { Format = ValueOutputFormat.MultipleLine };
                    for (int i = 0; i < columns.Length; i++)
                    {
                        object obj = row[i];
                        if (codeColumns.ContainsKey(columns[i]))
                            obj = new CodeString(obj.ToString());

                        instance.AddProperty(columns[i], new Value(obj));
                    }
                    L.Add(new KeyValuePair<object, object>(key, instance));
                }
                else
                {
                    object obj = row[1];
                    if (codeColumns.ContainsKey(columns[1]))
                        obj = new CodeString(obj.ToString());

                    L.Add(new KeyValuePair<object, object>(key, new Value(obj)));
                }
            }

            var groups = L.GroupBy(x => x.Key, x => x.Value);
            Dictionary<object, object> dict = new Dictionary<object, object>();
            foreach (var group in groups)
            {
                var A = group.ToArray();
                if (A.Length > 1)
                {
                    valueType.IsArray = true;
                    break;
                }
            }

            foreach (var group in groups)
            {
                var A = group.ToArray();
                object val;
                if (valueType.IsArray)
                    val = new Value(A) { Type = valueType };
                else
                    val = A[0];

                dict.Add(group.Key, val);
            }


            TypeInfo typeinfo = new TypeInfo { UserType = $"Dictionary<{keyType}, {valueType}>" };
            Field field = new Field(typeinfo, fieldName, new Value(dict) { Type = typeinfo })
            {
                Modifier = Modifier.Public | Modifier.Static | Modifier.Readonly
            };

            return field;
        }

        private static Field CreateListOrArrayField(string fieldName, DataClassType dataType, DataTable dt, string cname, string[] columns, IDictionary<string, TypeInfo> codeColumns)
        {

            List<Value> L = new List<Value>();
            TypeInfo type = new TypeInfo { UserType = $"{cname}" };
            foreach (DataRow row in dt.Rows)
            {
                var instance = new New(type) { Format = ValueOutputFormat.MultipleLine };
                for (int i = 0; i < columns.Length; i++)
                {
                    object obj = row[i];
                    if (codeColumns.ContainsKey(columns[i]))
                        obj = new CodeString(obj.ToString());

                    instance.AddProperty(columns[i], new Value(obj));
                }
                L.Add(instance);
            }

            TypeInfo typeinfo = new TypeInfo { UserType = $"{cname}[]" };
            if (dataType == DataClassType.List)
                typeinfo = new TypeInfo { UserType = $"List<{cname}>" };

            Field field = new Field(typeinfo, fieldName, new Value(L.ToArray()) { Type = typeinfo })
            {
                Modifier = Modifier.Public | Modifier.Static | Modifier.Readonly
            };

            return field;
        }


        private void ExportEnum(DataTable dt)
        {
            int count = dt.Columns.Count;
            if (count < 2)
            {
                cerr.WriteLine("cannot generate enum class because table is < 2 columns");
                return;
            }

            CSharpBuilder builder = new CSharpBuilder()
            {
                Namespace = NamespaceName
            };
            builder.AddUsingRange(base.Usings);

            string cname = ClassName;
            if (count > 2)
                builder.AddUsing("System.ComponentModel");

            DataColumn _feature = null;     //1st string column as property name
            DataColumn _value = null;       //1st int column as property value
            DataColumn _label = null;       //2nd string column as attribute [DataEnum("label")]
            DataColumn _category = null;    //3rd string column as category to generate multiple enum types
            foreach (DataColumn column in dt.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    if (_feature == null)
                        _feature = column;
                    else if (_label == null)
                        _label = column;
                    else if (_category == null)
                        _category = column;
                }

                if (_value == null && column.DataType == typeof(int))
                    _value = column;
            }

            if (_feature == null)
            {
                cerr.WriteLine("invalid enum property name");
                return;
            }

            if (_value == null)
            {
                cerr.WriteLine("invalid enum property value");
                return;
            }

            var rows = dt
                .AsEnumerable()
                .Select(row => new
                {
                    Feature = row.Field<string>(_feature),
                    Value = row.Field<int>(_value),
                    Category = _category != null ? row.Field<string>(_category) : null,
                    Label = _label != null ? row.Field<string>(_label) : null
                });

            if (_category != null)
            {
                var groups = rows.GroupBy(row => row.Category);

                foreach (var group in groups)
                {
                    var _enum = new EnumType(group.First().Category);
                    foreach (var row in group)
                        _enum.Add(row.Feature, row.Value, $"\"{row.Label}\"");

                    builder.AddEnum(_enum);
                }
            }
            else
            {
                var _enum = new EnumType(cname);
                foreach (var row in rows)
                    _enum.Add(row.Feature, row.Value, $"\"{row.Label}\"");

                builder.AddEnum(_enum);
            }

            PrintOutput(builder, cname);

        }


        private void ExportConstant(DataTable dt)
        {
            //command: export /c# /type:const /field:col1,col2 /value:col3,col4
            string[] optionColumns = cmd.GetStringArray("field");
            string[] optionConstants = cmd.GetStringArray("value");

            if (optionColumns.Length == 0)
            {
                cerr.WriteLine("missing parameter /field:col1,col2");
                return;
            }

            if (optionConstants.Length == 0)
            {
                optionConstants = optionColumns;
            }
            else if (optionColumns.Length != optionConstants.Length)
            {
                cerr.WriteLine($"invalid parameter /value:{string.Join(",", optionConstants)}");
                return;
            }

            CSharpBuilder builder = new CSharpBuilder()
            {
                Namespace = NamespaceName
            };
            builder.AddUsingRange(base.Usings);

            string cname = ClassName;
            Class clss = new Class(cname)
            {
                Modifier = Modifier.Public | Modifier.Static
            };
            builder.AddClass(clss);

            SortedDictionary<string, object> dict = new SortedDictionary<string, object>();
            Type type = null;

            int i = 0;
            foreach (string column in optionColumns)
            {
                string constant = optionConstants[i++];

                Type _type = dt.Columns[constant].DataType;
                if (type == null)
                {
                    type = _type;
                }
                else if (type != _type)
                {
                    cerr.WriteLine($"column [{constant}] data type is imcompatible");
                    continue;
                }

                foreach (DataRow row in dt.Rows)
                {
                    if (row[column] == DBNull.Value)
                        continue;

                    string key = row.Field<string>(column);
                    if (!dict.ContainsKey(key))
                        dict.Add(key, row[constant]);
                }
            }

            foreach (var kvp in dict)
            {
                string fieldName = Sys.ident.Identifier(kvp.Key);

                Field field = new Field(new TypeInfo(type), fieldName, new Value(kvp.Value))
                {
                    Modifier = Modifier.Public | Modifier.Const,
                    Comment = new Comment(kvp.Key),
                };

                clss.Add(field);
            }

            PrintOutput(builder, cname);
        }
    }


}
