﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Sys;
using Sys.Data;
using Sys.Data.Comparison;
using Sys.Stdio;

namespace sqlcli
{

    class CompareAdapter
    {

        public Side Side1 { get; private set; }
        public Side Side2 { get; private set; }

        private readonly ApplicationCommand cmd;

        public CompareAdapter(ApplicationCommand cmd, Side side1, Side side2)
        {
            this.cmd = cmd;

            this.Side1 = side1;
            this.Side2 = side2;
        }

        private static bool Exists(TableName tname)
        {
            if (!tname.Exists())
            {
                cout.WriteLine("table not found : {0}", tname);
                return false;
            }

            return true;
        }

        private static bool Exists(DatabaseName dname)
        {
            if (!dname.Exists())
            {
                cout.WriteLine("table not found : {0}", dname);
                return false;
            }

            return true;
        }


        public string Run(ActionType compareType, TableName[] N1, TableName[] N2, ApplicationCommand cmd)
        {
            string[] exceptColumns = cmd.Columns;

            DatabaseName dname1 = Side1.DatabaseName;
            DatabaseName dname2 = Side2.DatabaseName;

            cout.WriteLine("server1: {0} default database:{1}", Side1.Provider.DataSource, dname1.Name);
            cout.WriteLine("server2: {0} default database:{1}", Side2.Provider.DataSource, dname2.Name);

            if (!Exists(dname1) || !Exists(dname2))
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("-- sqlcli:", Side1.Provider.DataSource, dname1.Name).AppendLine();
            builder.AppendFormat("-- compare server={0} db={1}", Side1.Provider.DataSource, dname1.Name).AppendLine();
            builder.AppendFormat("--         server={0} db={1} @ {2}", Side2.Provider.DataSource, dname2.Name, DateTime.Now).AppendLine();

            Wildcard<TableName> match = MatchedDatabase.CreateWildcard(cmd);

            CancelableWork.CanCancel(cts =>
            {
                int i = 0;
                foreach (var tname1 in N1)
                {
                    if (cts.IsCancellationRequested)
                        return;

                    TableName tname2 = N2.Where(t => t.ShortName == tname1.ShortName).FirstOrDefault();
                    if (tname2 == null)
                    {
                        //when compare tables in the same database, the table name could be different
                        if (i < N2.Length && N2[i].DatabaseName == tname1.DatabaseName)
                            tname2 = N2[i];
                        else
                            tname2 = new TableName(dname2, tname1.SchemaName, tname1.Name);
                    }

                    if (compareType == ActionType.CompareData && !match.Contains(tname1))
                    {
                        cout.WriteLine("{0} is excluded", tname1);
                        continue;
                    }

                    if (tname2.Exists())
                    {
                        try
                        {
                            builder.Append(CompareTable(compareType, CompareSideType.compare, tname1, tname2, cmd.PK, exceptColumns));
                        }
                        catch (Exception ex)
                        {
                            cerr.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        if (compareType == ActionType.CompareSchema)
                        {
                            string sql = tname1.GenerateCreateTableClause(appendGO: false);
                            cout.WriteLine(sql);
                            builder
                            .Append(sql)
                            .AppendLine(SqlScript.GO);
                        }
                        else
                        {
                            cout.WriteLine("{0} doesn't exist", tname2);
                        }
                    }

                    i++;
                }

            });

            return builder.ToString();
        }

        private string CompareDatabaseSchema(CompareSideType sideType, DatabaseName db1, DatabaseName db2)
        {
            cout.WriteLine("{0} database schema {1} => {2}", sideType, db1.Name, db2.Name);
            return Compare.DatabaseSchemaDifference(sideType, db1, db2);
        }

        private string CompareDatabaseData(CompareSideType sideType, DatabaseName db1, DatabaseName db2, string[] excludedtables)
        {
            cout.WriteLine("compare database data {0} => {1}", db1.Name, db2.Name);
            if (excludedtables != null && excludedtables.Length > 0)
                cout.WriteLine("ignore tables: {0}", string.Join(",", excludedtables));
            return Compare.DatabaseDifference(sideType, db1, db2, excludedtables);
        }

        public string CompareTable(ActionType actiontype, CompareSideType sidetype, TableName tname1, TableName tname2, IDictionary<string, string[]> pk, string[] exceptColumns)
        {
            if (!Exists(tname1))
            {
                return string.Empty;
            }

            if (actiontype == ActionType.CompareRowCount)
            {
                return CompareRowCount(tname1, tname2);
            }

            TableSchema schema1 = new TableSchema(tname1);
            TableSchema schema2 = new TableSchema(tname2);

            string sql = string.Empty;

            if (actiontype == ActionType.CompareSchema)
            {
                sql = Compare.TableSchemaDifference(sidetype, tname1, tname2);
                cout.WriteLine("completed to {0} table schema {1} => {2}", sidetype, tname1, tname2);
            }
            else if (actiontype == ActionType.CompareData)
            {
                if (!Exists(tname2))
                {
                    return string.Empty;
                }

                if (Compare.TableSchemaDifference(sidetype, tname1, tname2) != string.Empty)
                {
                    cout.WriteLine("failed to {0} becuase of different table schemas", sidetype);
                    return string.Empty;
                }

                sql = CompareData(sidetype, schema1, tname1, schema2, tname2, pk, exceptColumns);
            }

            if (sql != string.Empty && sidetype == CompareSideType.compare)
                cout.WriteLine(sql);

            return sql;
        }

        private static string CompareRowCount(TableName tname1, TableName tname2)
        {
            string text = string.Empty;
            if (!Exists(tname2))
            {
                text = $"warning: {tname2} doesn't exist";
                cout.WriteLine(ConsoleColor.DarkRed, text);
                return text;
            }

            long count1 = new TableReader(tname1).MaxCount;
            long count2 = new TableReader(tname2).MaxCount;

            if (count1 != count2)
            {
                text = $"{tname1} => {tname2} count={count1} != {count2}";
                cout.WriteLine(ConsoleColor.Red, $"completed table count {text}");
                return text + Environment.NewLine;
            }
            else
                cout.WriteLine($"completed table count {tname1} => {tname2} count={count1}");

            return text;
        }

        private static string CompareData(CompareSideType sidetype, TableSchema schema1, TableName tname1, TableSchema schema2, TableName tname2, IDictionary<string, string[]> pk, string[] exceptColumns)
        {
            string sql;
            bool hasPk = schema1.PrimaryKeys.Length > 0;
            sql = Compare.TableDifference(sidetype, schema1, schema2, schema1.PrimaryKeys.Keys, exceptColumns);

            if (!hasPk)
            {
                cout.WriteLine("warning: no primary key found : {0}", tname1);

                string key = tname1.Name.ToUpper();
                if (pk.ContainsKey(key))
                {
                    cout.WriteLine("use predefine keys defined in ini file: {0}", tname1);
                    sql = Compare.TableDifference(sidetype, schema1, schema2, pk[key], exceptColumns);
                }
                else
                {
                    cout.WriteLine("use entire row as primary keys:{0}", tname1);
                    var keys = schema1.Columns.Select(row => row.ColumnName).ToArray();
                    sql = Compare.TableDifference(sidetype, schema1, schema2, keys, exceptColumns);
                }
            }

            cout.WriteLine("completed to {0} table data {1} => {2}", sidetype, tname1, tname2);
            return sql;
        }
    }
}
