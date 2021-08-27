using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sys.Data
{
	static class Extension
	{
		public static T IsNull<T>(this object value, T defaultValue)
		{
			if (value is T)
				return (T)value;

			if (value == null || value == DBNull.Value)
				return defaultValue;

			throw new Exception($"{value} is not type of {typeof(T)}");
		}

		public static T GetField<T>(this DataRow row, string columnName, T defaultValue = default(T))
		{
			if (!row.Table.Columns.Contains(columnName))
				return defaultValue;

			return IsNull<T>(row[columnName], defaultValue);
		}

		public static List<T> ToList<T>(this DataTable dt, Func<DataRow, T> select)
		{
			List<T> list = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				list.Add(select(row));
			}

			return list;
		}


	}
}
