﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tie;

namespace Sys.Data
{
    class SqlClauseParser
    {
        private readonly ConnectionProvider provider;
        private readonly string sql;

        public SqlClauseParser(ConnectionProvider provider, string sql)
        {
            this.provider = provider;
            this.sql = ToUpperCase(sql);
        }

        public SqlClause Parse()
        {
            return new UpdateClause
            {
                ClauseAction = SqlClauseAction.Update
            };
        }

        public UpdateClause ParseUpdate()
        {
            return new UpdateClause
            {
                ClauseAction = SqlClauseAction.Update
            };
        }

        public InsertClause ParseInsert()
        {
            return new InsertClause
            {
                ClauseAction = SqlClauseAction.Insert
            };
        }

        public DeleteClause ParseDelete()
        {
            return new DeleteClause
            {
                ClauseAction = SqlClauseAction.Delete
            };
        }

        public SelectClause ParseSelect()
        {
            string[] items = sql.Split(new string[] { "SELECT", "FROM", "WHERE" }, StringSplitOptions.RemoveEmptyEntries);

            string name = null;
            string where = null;
            string columns = null;

            int top = 0;
            if (items.Length > 0)
            {
                columns = items[0].Trim();
                top = ParseColumns(columns);
            }

            if (items.Length > 1)
                name = items[1].Trim();
            else
                throw new InvalidDataException($"cannot extract table name from SQL:{sql}");

            if (items.Length > 2)
                where = items[2].Trim();

            var tname = new TableName(provider, name);

            SelectClause select = new SelectClause
            {
                Top = top,
                TableName = tname,
                Locator = new Locator(where),
            };

            return select;
        }

        private int ParseColumns(string columns)
        {
            int top = 0;
            var lexer = new SqlTokenizer(columns);

            var token = lexer.GetNextToken();
            if (token.ty == tokty.identsy && token.tok == "TOP")
            {
                if (!lexer.ExpectInt32(out top))
                    throw new Exception($"integer expected after keyword TOP, {columns}");
            }


            return top;
        }



        private static readonly string[] keywords = new string[]
        {
            "SELECT",
            "TOP",
            "AS",
            "FROM",
            "WHERE",

            "UPDATE",
            "SET",
            "DELETE",

            "INSERT",
            "INTO",
            "VALUES",
        };

        private static string ToUpperCase(string sql)
        {
            string[] items = sql.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < items.Length; i++)
            {
                foreach (string keyword in keywords)
                {
                    if (string.Compare(items[i], keyword, ignoreCase: true) == 0)
                        items[i] = keyword;
                }
            }

            return string.Join(" ", items);
        }
    }
}
