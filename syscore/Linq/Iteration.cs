using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;

namespace Sys
{
	public static class Iteration
	{
        public static List<T> ToList<T>(this DataRow row, Func<int, object, T> func)
        {
            List<T> list = new List<T>();
            int i = 0;
            foreach (var obj in row.ItemArray)
            {
                list.Add(func(i, obj));
                i++;
            }

            return list;
        }

        public static List<T> ToList<T>(this DataRow row, Func<object, T> func)
        {
            List<T> list = new List<T>();
            foreach (var obj in row.ItemArray)
            {
                list.Add(func(obj));
            }

            return list;
        }

        public static List<T> ToList<T>(this DataTable dt, Func<int, DataRow, bool> where, Func<int, DataRow, T> select)
        {
            List<T> list = new List<T>();
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (where(i, row))
                    list.Add(select(i, row));
                i++;
            }

            return list;
        }

        public static List<T> ToList<T>(this DataTable dt, Func<DataRow, bool> where, Func<DataRow, T> select)
        {
            return dt.ToList((_, row) => where(row), (_, row) => select(row));
        }

        public static List<T> ToList<T>(this DataTable dt, Func<int, DataRow, T> select)
        {
            return dt.ToList((rowId, row) => true, (rowId, row) => select(rowId, row));
        }

        public static List<T> ToList<T>(this DataTable dt, Func<DataRow, T> select)
        {
            return dt.ToList((rowId, row) => true, (rowId, row) => select(row));
        }

        public static T[] ToArray<T>(this DataTable dataTable, string columnName)
        {
            return ToArray<T>(dataTable, row => (T)row[columnName]);
        }

        public static T[] ToArray<T>(this DataTable dataTable, int columnIndex = 0)
        {
            return ToArray<T>(dataTable, row => (T)row[columnIndex]);
        }

        public static T[] ToArray<T>(this DataTable dataTable, Func<DataRow, T> func)
        {
            T[] values = new T[dataTable.Rows.Count];

            int i = 0;
            foreach (DataRow row in dataTable.Rows)
            {
                values[i++] = func(row);
            }

            return values;
        }

        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            if (dt == null)
                return null;

            List<T> list = new List<T>();
            if (dt.Rows.Count == 0)
                return list;

            var properties = typeof(T).GetProperties();
            Dictionary<DataColumn, System.Reflection.PropertyInfo> d = new Dictionary<DataColumn, System.Reflection.PropertyInfo>();
            foreach (DataColumn column in dt.Columns)
            {
                var property = properties.FirstOrDefault(p => p.Name.ToUpper() == column.ColumnName.ToUpper());
                if (property != null)
                {
                    Type ct = column.DataType;
                    Type pt = property.PropertyType;

                    if (pt == ct || (pt.GetGenericTypeDefinition() == typeof(Nullable<>) && pt.GetGenericArguments()[0] == ct))
                        d.Add(column, property);
                }
            }

            foreach (DataRow row in dt.Rows)
            {
                T item = new T();
                foreach (DataColumn column in dt.Columns)
                {
                    if (d.ContainsKey(column))
                    {
                        var propertyInfo = d[column];
                        object obj = row[column];

                        if (obj != null && obj != DBNull.Value)
                            propertyInfo.SetValue(item, obj);
                    }
                }

                list.Add(item);
            }

            return list;
        }


        public static IEnumerable<DataLine> Where(this DataTable dt, Func<DataLine, bool> where)
        {
            return dt.ToLines().Where(where);
        }

        public static IEnumerable<DataLine> ToLines(this DataTable dt)
        {
            int i = 0;
            foreach (DataRow row in dt.Rows)
            {
                yield return new DataLine { Line = i, Row = row };
                i++;
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DataTable dataTable, Func<DataRow, TKey> keySelector, Func<DataRow, TValue> valueSelector)
        {
            Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

            foreach (DataRow row in dataTable.Rows)
            {
                TKey key = keySelector(row);
                TValue value = valueSelector(row);
                if (dict.ContainsKey(key))
                    dict[key] = value;
                else
                    dict.Add(key, value);
            }

            return dict;
        }

    }
}
