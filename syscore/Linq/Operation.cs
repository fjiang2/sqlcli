using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Sys.Data
{
	public static class Operation
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

		public static void Insert(this DataTable dt, Action<DataRow> insert)
		{
			DataRow row = dt.NewRow();
			insert(row);
			dt.Rows.Add(row);

			dt.AcceptChanges();
		}

		public static int Update(this DataTable dt, Action<DataRow> update)
		{
			return Update(dt, (_, row) => true, (_, row) => update(row));
		}

		public static int Update(this DataTable dt, Func<DataRow, bool> where, Action<DataRow> update)
		{
			return Update(dt, (_, row) => where(row), (_, row) => update(row));
		}

		public static int Update(this DataTable dt, Action<int, DataRow> update)
		{
			return Update(dt, (_, row) => true, update);
		}

		public static int Update(this DataTable dt, Func<int, DataRow, bool> where, Action<int, DataRow> update)
		{
			int count = 0;
			int i = 0;
			foreach (DataRow row in dt.Rows)
			{
				if (where(i, row))
				{
					update(i, row);
					count++;
				}
				i++;
			}

			return count;
		}

		public static int Delete(this DataTable dt, Func<DataRow, bool> where)
		{
			return Delete(dt, (_, row) => where(row));
		}

		public static int Delete(this DataTable dt, Func<int, DataRow, bool> where)
		{
			int count = 0;
			int i = 0;
			foreach (DataRow row in dt.Rows)
			{
				if (where(i, row))
				{
					row.Delete();
					count++;
				}
				i++;
			}

			dt.AcceptChanges();
			return count;
		}

		public static DataTable ToDataTable<T>(this IEnumerable<T> source)
		{
			var properties = typeof(T).GetProperties();

			DataTable dt = new DataTable();
			foreach (var propertyInfo in properties)
			{
				dt.Columns.Add(new DataColumn(propertyInfo.Name, propertyInfo.PropertyType));
			}

			Func<T, object[]> selector = row =>
			{
				var values = new object[properties.Length];
				int i = 0;

				foreach (var propertyInfo in properties)
				{
					values[i++] = propertyInfo.GetValue(row);
				}

				return values;
			};

			foreach (T row in source)
			{
				object[] values = selector(row);
				var newRow = dt.NewRow();
				int k = 0;
				foreach (var item in values)
				{
					newRow[k++] = item;
				}

				dt.Rows.Add(newRow);
			}

			dt.AcceptChanges();
			return dt;
		}

		public static DataSet ToDataSet(this string xml)
		{
			DataSet ds = new DataSet();
			return ToDataSet(xml, ds);
		}

		public static DataSet ToDataSet(this string xml, DataSet ds)
		{
			using (MemoryStream stream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.Write(xml);
				writer.Flush();
				stream.Position = 0;

				try
				{
					ds.ReadXml(stream, XmlReadMode.ReadSchema);
				}
				catch (Exception)
				{
					throw new Exception(xml);
				}
			}
			return ds;
		}


		public static string ToXml(this DataSet ds)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				ds.WriteXml(stream, XmlWriteMode.WriteSchema);
				stream.Flush();
				stream.Position = 0;

				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}

		public static DataTable ToDataTable(this string xml, DataTable dt)
		{
			using (MemoryStream stream = new MemoryStream())
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.Write(xml);
				writer.Flush();
				stream.Position = 0;

				try
				{
					dt.ReadXml(stream);
				}
				catch (Exception)
				{
					throw new Exception(xml);
				}
			}
			return dt;
		}


		public static string ToXml(this DataTable dt)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				dt.WriteXml(stream, XmlWriteMode.WriteSchema);
				stream.Flush();
				stream.Position = 0;

				using (StreamReader reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}
