using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using syscon.stdio;

namespace syscon.grid
{
    public static class DataGrid
    {
        public static void ToGrid<T>(this IEnumerable<T> source, bool vertical = false)
        {
            DataTable dt = source.ToDataTable();
            new OutputDataTable(dt, cout.TrimWriteLine, vertical).Output();
        }

        public static void ToGrid(this DataTable dt, bool vertical = false, bool more = false, bool outputDbNull = true, int maxColumnWidth = 0)
        {
            OutputDataTable odt = new OutputDataTable(dt, cout.TrimWriteLine, vertical)
            {
                OutputDbNull = outputDbNull,
                MaxColumnWidth = maxColumnWidth,
            };
            odt.Output();

            var top = more ? "top " : "";
            var rows = dt.Rows.Count > 1 ? "rows" : "row";

            cout.WriteLine($"<{top}{dt.Rows.Count} {rows}>");
        }


        public static void ToGrid(this DbDataReader reader, int maxRow = 0)
        {
            while (reader.HasRows)
            {
                DataTable schemaTable = reader.GetSchemaTable();

                var schema = schemaTable
                    .Rows.OfType<DataRow>()
                    .Select(row => new
                    {
                        Name = (string)row["ColumnName"],
                        Size = (int)row["ColumnSize"],
                        Type = (Type)row["DataType"]
                    });

                string[] headers = schema.Select(row => row.Name).ToArray();

                var D = new OutputDataLine(cout.TrimWriteLine, headers.Length);

                D.MeasureWidth(schema.Select(row => row.Size).ToArray());
                D.MeasureWidth(headers);
                D.MeasureWidth(schema.Select(row => row.Type).ToArray());

                D.DisplayLine();
                D.DisplayLine(headers);
                D.DisplayLine();

                if (!reader.HasRows)
                {
                    cout.WriteLine("<0 row>");
                    return;
                }

                object[] values = new object[headers.Length];
                int count = 0;
                bool limited = false;
                while (reader.Read())
                {
                    reader.GetValues(values);
                    D.DisplayLine(values);

                    if (++count == maxRow)
                    {
                        limited = true;
                        break;
                    }

                }

                D.DisplayLine();

                var rows = count > 1 ? "rows" : "row";
                var limit = limited ? "limit reached" : "";
                cout.WriteLine($"<{count} {rows}> {limit}");

                reader.NextResult();
            }

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

    }
}
