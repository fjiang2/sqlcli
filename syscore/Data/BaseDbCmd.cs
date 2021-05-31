using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Diagnostics.Contracts;

namespace Sys.Data
{
    public abstract class BaseDbCmd : IDbFill, IDbCmd
    {
        public BaseDbCmd()
        {
        }

        public abstract DataSet FillDataSet(DataSet dataSet);
        public abstract int ExecuteNonQuery();
        public abstract object ExecuteScalar();

        public DataSet FillDataSet()
        {

            DataSet ds = new DataSet();

            if (FillDataSet(ds) == null)
                return null;

            return ds;
        }

        public DataTable FillDataTable()
        {
            DataSet ds = FillDataSet();
            if (ds == null)
                return null;

            if (ds.Tables.Count >= 1)
                return ds.Tables[0];

            return null;
        }

        public IEnumerable<T> FillDataColumn<T>(int column = 0)
        {
            Contract.Requires(column >= 0);

            List<T> list = new List<T>();

            DataTable table = FillDataTable();
            if (table == null)
                return list;

            foreach (DataRow row in table.Rows)
            {
                object obj = row[column];
                list.Add(ToObject<T>(obj));
            }

            return list;
        }

        public IEnumerable<T> FillDataColumn<T>(string columnName)
        {
            Contract.Requires(!string.IsNullOrEmpty(columnName));

            List<T> list = new List<T>();

            DataTable table = FillDataTable();
            if (table == null)
                return list;

            foreach (DataRow row in table.Rows)
            {
                object obj = row[columnName];
                list.Add(ToObject<T>(obj));
            }

            return list;
        }

        public DataRow FillDataRow()
        {
            return FillDataRow(0);
        }

        public DataRow FillDataRow(int row = 0)
        {
            Contract.Requires(row >= 0);

            DataTable table = FillDataTable();
            if (table != null && row < table.Rows.Count)
                return table.Rows[row];
            else
                return null;
        }

        public object FillObject()
        {
            DataRow row = FillDataRow();
            if (row != null && row.Table.Columns.Count > 0)
                return row[0];
            else
                return null;
        }

        public T FillObject<T>()
        {
            var obj = FillObject();

            return ToObject<T>(obj);
        }

        private static T ToObject<T>(object obj)
        {
            if (obj != null && obj != DBNull.Value)
                return (T)obj;
            else
                return default(T);
        }

        public List<T> ToList<T>(Func<DataRow, T> newObject)
        {
            var dt = FillDataTable();
            if (dt == null)
                return null;

            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(newObject(row));
            }

            return list;
        }
    }
}
