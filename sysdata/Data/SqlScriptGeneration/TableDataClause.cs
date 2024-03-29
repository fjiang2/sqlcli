﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using Sys.Data.Text;

namespace Sys.Data
{
    class TableDataClause
    {
        private readonly ITableSchema schema;
        private readonly TableName tableName;
        private readonly SqlTemplate template;
        private readonly string[] pk;
        private readonly string[] ik;
        private readonly string[] ck;


        public TableDataClause(ITableSchema schema)
        {
            this.schema = schema;
            this.tableName = schema.TableName;
            this.template = new SqlTemplate(tableName.FormalName);

            this.pk = schema.PrimaryKeys.Keys;
            this.ik = schema.Identity.ColumnNames;
            this.ck = schema.Columns.Where(column => column.IsComputed).Select(column => column.ColumnName).ToArray();
        }


        private string WHERE(ColumnPairCollection pairs)
        {
            var L1 = pairs.Where(p => pk.Contains(p.ColumnName)).ToArray();
            return string.Join<ColumnPair>(" AND ", L1);
        }

        public string IF_NOT_EXISTS_INSERT(ColumnPairCollection pairs)
        {
            return template.IfNotExistsInsert(WHERE(pairs), INSERT(pairs));
        }

        public string INSERT(ColumnPairCollection pairs, bool InsertWithoutColumns = false)
        {
            var L1 = pairs
              .Where(column => !ik.Contains(column.ColumnName))
              .Where(column => !ck.Contains(column.ColumnName));

            var x1 = L1.Select(p => p.ColumnName.AsColumn());
            var x2 = L1.Select(p => p.Value.ToScript());

            if (InsertWithoutColumns)
                return template.Insert(string.Join(",", x2));
            else
                return template.Insert(string.Join(",", x1), string.Join(",", x2));
        }


        public string IF_NOT_EXISTS_INSERT_ELSE_UPDATE(ColumnPairCollection pairs)
        {
            return template.IfNotExistsInsertElseUpdate(WHERE(pairs), INSERT(pairs), UPDATE(pairs));
        }

        public string UPDATE(ColumnPairCollection pairs)
        {
            var L1 = pairs
                .Where(column => !ik.Contains(column.ColumnName))
                .Where(column => !pk.Contains(column.ColumnName))
                .Where(column => !ck.Contains(column.ColumnName))
                .Select(p => $"{p.ColumnName.AsColumn()} = {p.Value.ToScript()}");

            string update = string.Join(",", L1);
            return template.Update(update, WHERE(pairs));
        }

        public string DELETE(DataRow row, IPrimaryKeys primaryKey)
        {
            var L1 = new List<ColumnPair>();
            foreach (var column in primaryKey.Keys)
            {
                L1.Add(new ColumnPair(column, row[column]));
            }

            return template.Delete(string.Join<ColumnPair>(" AND ", L1));
        }

    }
}
