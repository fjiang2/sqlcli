﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Sys
{
	public static class Operation
	{
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
    }
}
