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
        public static void ToConsole(this DbDataReader reader, int maxRow = 0)
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
                cout.WriteLine("<{0} row{1}> {2}",
                    count,
                    count > 1 ? "s" : "",
                    limited ? "limit reached" : ""
                    );

                reader.NextResult();
            }

        }
    }
}
