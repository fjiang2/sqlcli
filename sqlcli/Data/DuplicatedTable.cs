using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sys.Data;
using syscon.stdio;
using Sys.Data.Text;

namespace sqlcli
{
    class DuplicatedTable
    {
        public bool AllColumnsSelected { get; } = false;

        private readonly TableName tname;
        private readonly string[] _columns;
        const string COUNT_COLUMN_NAME = "$Count";
        public DataTable group { get; }

        public DuplicatedTable(TableName tname, string[] columns)
        {
            this.tname = tname;

            if (columns.Length == 0)
            {
                CType[] ctypes = new CType[] 
                { 
                    CType.Image, 
                    CType.Text,
                    CType.NText,
                    CType.Xml 
                };

                _columns = new TableSchema(tname)
                    .Columns
                    .Where(column => !ctypes.Contains(column.CType))
                    .Select(column => column.ColumnName)
                    .ToArray();

                AllColumnsSelected = true;
            }
            else
                this._columns = columns;

            var builder = new SqlBuilder()
                .SELECT()
                .COLUMNS($"COUNT(*) AS [{COUNT_COLUMN_NAME}],")
                .COLUMNS(_columns)
                .FROM(tname)
                .GROUP_BY(_columns).HAVING(Expression.COUNT_STAR > 1 )
                .ORDER_BY(_columns);

            group = new SqlCmd(tname.Provider, builder).FillDataTable();
        
        }

        public void Dispaly(Action<DataTable> display)
        {
            foreach (var row in group.AsEnumerable())
            {
                var where = _columns.Select(column => column.AssignColumn(row[column])).AND();
                if (AllColumnsSelected)
                    Cout.WriteLine("idential rows");
                else
                    Cout.WriteLine("{0}", where);

                var builder = new SqlBuilder().SELECT().COLUMNS().FROM(tname).WHERE(where);
                display(new SqlCmd(tname.Provider, builder).FillDataTable());
                Cout.WriteLine();
            }
        }

        public int DuplicatedRowCount()
        {
            int sum = 0;
            foreach (var row in group.AsEnumerable())
            {
                int count = row.Field<int>(COUNT_COLUMN_NAME);
                sum += count - 1;
            }

            return sum;
        }


        public int Clean()
        {
            int sum = 0;
            foreach (var row in group.AsEnumerable())
            {
                int count = row.Field<int>(COUNT_COLUMN_NAME);

                var where = _columns.Select(column => column.AssignColumn(row[column])).AND();
                var builder = new SqlBuilder()
                    .SET("ROWCOUNT", count-1)
                    .DELETE_FROM(tname)
                    .WHERE(where)
                    .SET("ROWCOUNT", 0);

                sum += count - 1;
                new SqlCmd(tname.Provider, builder).ExecuteNonQuery();
            }

            return sum;
        }
    }
}
