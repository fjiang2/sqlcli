//--------------------------------------------------------------------------------------------------//
//                                                                                                  //
//        DPO(Data Persistent Object)                                                               //
//                                                                                                  //
//          Copyright(c) Datum Connect Inc.                                                         //
//                                                                                                  //
// This source code is subject to terms and conditions of the Datum Connect Software License. A     //
// copy of the license can be found in the License.html file at the root of this distribution. If   //
// you cannot locate the  Datum Connect Software License, please send an email to                   //
// datconn@gmail.com. By using this source code in any fashion, you are agreeing to be bound        //
// by the terms of the Datum Connect Software License.                                              //
//                                                                                                  //
// You must not remove this notice, or any other, from this software.                               //
//                                                                                                  //
//                                                                                                  //
//--------------------------------------------------------------------------------------------------//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Threading;
using Sys.Data.Text;

namespace Sys.Data
{
    /// <summary>
    /// Read records from data base server into data table 
    /// </summary>
    public class TableReader
    {
        private readonly TableName tableName;
        private readonly Lazy<DataTable> table;
        private readonly Locator locator;

        public bool CaseSensitive { get; set; } = false;
        public int Top { get; set; }

        public TableReader(TableName tableName, Locator locator)
        {
            this.table = new Lazy<DataTable>(() => LoadData());
            this.tableName = tableName;
            this.locator = locator;
        }


        /// <summary>
        /// read all records in the table defined
        /// </summary>
        /// <param name="tableName"></param>
        public TableReader(TableName tableName)
            : this(tableName, new Locator())
        {
        }

        internal DbCmd Command
        {
            get
            {
                var sql = new SqlBuilder().SELECT().TOP(Top).COLUMNS().FROM(tableName).WHERE(locator);
                return new SqlCmd(tableName.Provider, sql.Script);
            }
        }

        /// <summary>
        /// Count of selected rows
        /// </summary>
        public long Count
        {
            get
            {
                if (table.IsValueCreated)
                    return Table.Rows.Count;

                var sql = new SqlBuilder().SELECT().COLUMNS(Expression.COUNT_STAR).FROM(tableName).WHERE(locator);

                object obj = new SqlCmd(tableName.Provider, sql.Script).ExecuteScalar();
                long count = Convert.ToInt64(obj);
                if (Top > 0 && Top < count)
                    return Top;
                else
                    return count;
            }
        }

        /// <summary>
        /// Count of all rows in table
        /// </summary>
        public long MaxCount
        {
            get
            {
                SqlBuilder query;
                if (tableName.Provider.Type == ConnectionProviderType.SqlServer)
                {
                    query = new SqlBuilder().SELECT().COLUMNS("CONVERT(bigint, rows)").FROM("sysindexes").WHERE($"id = OBJECT_ID('{tableName.ShortName}') AND indid < 2");
                }
                else
                {
                    query = new SqlBuilder().SELECT().COLUMNS("COUNT(*)").FROM(tableName);
                }

                object obj = new SqlCmd(tableName.Provider, query.ToString()).ExecuteScalar();
                return Convert.ToInt64(obj);
            }
        }

        /// <summary>
        /// return data table retrieved from data base server
        /// </summary>
        public DataTable Table => table.Value;

        private DataTable LoadData()
        {
            DataTable dt = Command.FillDataTable();
            dt.CaseSensitive = CaseSensitive;
            var schema = new TableSchema(tableName);
            string[] keys = schema.PrimaryKeys.Keys;
            dt.PrimaryKey = dt.Columns.OfType<DataColumn>().Where(column => keys.Contains(column.ColumnName)).ToArray();
            foreach (IColumn column in schema.Columns)
            {
                DataColumn _column = dt.Columns[column.ColumnName];
                _column.AllowDBNull = column.Nullable;
                _column.AutoIncrement = column.IsIdentity;

                //because string supports Unicode
                if (column.CType == CType.NVarChar || column.CType == CType.NChar)
                {
                    if (column.Length > 0)
                        _column.MaxLength = column.Length / 2;
                }
            }

            return dt;
        }

        /// <summary>
        /// override T.Fill(DataRow) to initialize varibles in the class T, if typeof(T).BaseType != typeof(DPObject)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ToList<T>() where T : class, IDPObject, new()
        {
            return new DPList<T>(this).ToList();
        }


        /// <summary>
        /// returns SQL clause
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Command.ToString();
        }

    }


}
