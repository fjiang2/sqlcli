﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.CodeBuilder;
using Sys.Data.Code;

namespace Sys.Data.Manager
{

    public static class Extension
    {

        public static string ToFieldName(this IColumn column, CodeStyle style = CodeStyle.Original)
        {
            string columnName = column.ColumnName;
            return ToFieldName(columnName, style);
        }

        public static string ToFieldName(this string columnName, CodeStyle style = CodeStyle.Original)
            => ToFieldName(columnName, "_", style);

        public static string ToFieldName(this string columnName, string prefix, CodeStyle style = CodeStyle.Original)
        {
            string fieldName = columnName;
            if (columnName.IndexOf("#") != -1
                || columnName.IndexOf(" ") != -1
                || columnName.IndexOf("/") != -1
                || !Char.IsLetter(columnName[0]))
            {
                fieldName = columnName.Replace("#", "_").Replace(" ", "_").Replace("/", "_");

                if (!Char.IsLetter(columnName[0]))
                    fieldName = prefix + fieldName;
            }

            char ch = fieldName[0];
            switch (style)
            {
                case CodeStyle.Pascal:
                    if (char.IsLower(ch))
                        return char.ToUpper(ch) + fieldName.Substring(1);
                    break;

                case CodeStyle.Camel:
                    if (char.IsUpper(ch))
                        return char.ToLower(ch) + fieldName.Substring(1);
                    break;
            }

            return fieldName;
        }



        public static string ToClassName(this TableName tname, Func<string, string> rule)
        {
            string tableName = tname.Name;
            string className = ident.Identifier(tableName);

            //remove plural
            className = Plural.Singularize(className);

            if (rule != null)
                className = rule(className);

            return className;

        }

       
    }
}
