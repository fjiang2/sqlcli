using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using System.Threading;

namespace Sys.Data
{
    class DbReader
    {
        private readonly DbDataReader reader;
        public int StartRecord { get; set; } = 0;
        public int MaxRecords { get; set; } = -1;


        public DbReader(DbDataReader reader)
        {
            this.reader = reader;
        }


        public DataRow ReadRow(DataTable table)
        {
            DataRow row = table.NewRow();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[i] = reader.GetValue(i);
            }

            return row;
        }

        public void ReadTable(CancellationToken cancellationToken, IProgress<DataRow> progress)
        {
            var table = CreateBlankTable(reader);

            while (reader.Read())
            {
                var row = ReadRow(table);
                progress.Report(row);

                if (cancellationToken.IsCancellationRequested)
                    break;
            }

        }


        public DataTable ReadTable(CancellationToken cancellationToken, IProgress<int> progress)
        {
            var table = CreateBlankTable(reader);

            int step = 0;

            while (reader.Read())
            {
                step++;
                progress?.Report(step);

                var row = ReadRow(table);
                table.Rows.Add(row);

                if (cancellationToken.IsCancellationRequested)
                    break;
            }

            table.AcceptChanges();

            return table;
        }

        public static DataTable CreateBlankTable(DbDataReader reader)
        {
            DataTable table = new DataTable
            {
                CaseSensitive = true,
            };

            CreateBlankTable(table, reader);
            return table;
        }

        private static void CreateBlankTable(DataTable table, DbDataReader reader)
        {
            if (table != null)
            {
                table.CaseSensitive = true;
                table.Columns.Clear();
                table.Clear();
                table.AcceptChanges();
            }
            else
            {
                table = new DataTable { CaseSensitive = true };
            }

            for (int i = 0; i < reader.FieldCount; i++)
            {
                DataColumn column = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                table.Columns.Add(column);
            }

            table.AcceptChanges();
        }


        public int ReadTable(DataTable table)
        {
            CreateBlankTable(table, reader);
            return ReadRows(table);
        }

        private int ReadRows(DataTable table)
        {
            if (MaxRecords == 0)
                return 0;

            int index = -1;
            int count = 0;
            while (reader.Read())
            {
                index++;
                if (index < StartRecord)
                    continue;

                var row = ReadRow(table);
                table.Rows.Add(row);
                count++;

                if (MaxRecords > 0 && count >= MaxRecords)
                    break;
            }

            table.AcceptChanges();
            return count;
        }

        public int ReadDataSet(DataSet ds)
        {
            int count = 0;

            //read empty table
            var dt = new DataTable();
            CreateBlankTable(dt, reader);
            ds.Tables.Add(dt);

            while (reader.HasRows)
            {
                count += ReadRows(dt);
                if (reader.NextResult())
                {
                    //read next empty table
                    dt = new DataTable();
                    CreateBlankTable(dt, reader);
                    ds.Tables.Add(dt);
                }
            }

            return count;
        }
    }
}


