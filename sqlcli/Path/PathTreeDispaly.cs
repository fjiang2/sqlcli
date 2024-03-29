﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data;
using Sys;
using Sys.Data;
using Sys.Stdio;
using Tie;

namespace sqlcli
{

    partial class PathManager
    {
        private static void Assign(ApplicationCommand cmd, IEnumerable<string> values)
        {
            string vname = cmd.GetValue("let");
            if (vname == null)
                return;

            if (vname == string.Empty)
            {
                cerr.WriteLine("undefined variable name");
                return;
            }

            string[] A = values.ToArray();
            Context.DS.Add(vname, new VAL(A));
        }

        public void Display(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (DisplayServerNodes(pt, cmd)) return;
            if (DisplayDatabaseNodes(pt, cmd)) return;
            if (DisplayTableNodes(pt, cmd)) return;
            if (DisplayTableSubNodes(pt, cmd)) return;
            if (_DisplayLocatorNodes(pt, cmd)) return;
            if (DisplayViewNodes(pt, cmd)) return;
        }

        private bool DisplayServerNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (pt != RootNode)
                return false;

            int i = 0;
            int count = 0;
            int h = 0;
            CancelableWork.CanCancel(cts =>
            {
                List<string> values = new List<string>();
                foreach (var node in pt.Nodes)
                {
                    if (cts.IsCancellationRequested)
                        return;

                    ServerName sname = (ServerName)node.Item;
                    ++i;

                    if (IsMatch(cmd.Wildcard, sname.Path))
                    {
                        count++;
                        if (node.Nodes.Count == 0)
                        {
                            ExpandServerName(node, Refreshing);
                        }

                        cout.WriteLine("{0,4} {1,26} <SVR> {2,10} Databases", sub(i), sname.Path, sname.Disconnected ? "?" : node.Nodes.Count.ToString());
                        h = PagePause(cmd, ++h);

                        values.Add(sname.Path);
                    }
                }

                Assign(cmd, values);
                cout.WriteLine("\t{0} Server(s)", count);
            });

            return true;
        }

        private static bool DisplayDatabaseNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (!(pt.Item is ServerName))
                return false;

            ServerName sname = (ServerName)pt.Item;
            if (sname.Disconnected)
            {
                cout.WriteLine("\t? Database(s)");
            }
            else
            {
                int i = 0;
                int count = 0;
                int h = 0;

                List<string> values = new List<string>();
                foreach (var node in pt.Nodes)
                {
                    DatabaseName dname = (DatabaseName)node.Item;
                    ++i;

                    if (IsMatch(cmd.Wildcard, dname.Path))
                    {
                        count++;
                        if (node.Nodes.Count == 0)
                            ExpandDatabaseName(node, cmd.Refresh);

                        cout.WriteLine("{0,4} {1,26} <DB> {2,10} Tables/Views", sub(i), dname.Name, node.Nodes.Count);
                        h = PagePause(cmd, ++h);

                        values.Add(dname.Name);
                    }
                }

                Assign(cmd, values);
                cout.WriteLine("\t{0} Database(s)", count);
            }

            return true;
        }


        private static bool DisplayTableNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (!(pt.Item is DatabaseName))
                return false;

            DatabaseName dname = (DatabaseName)pt.Item;
            if (cmd.HasStorage)
            {
                displayTable(dname.StorageSchema(), "Storage");
                return true;
            }

            int i = 0;
            int[] count = new int[] { 0, 0, 0, 0 };
            int h = 0;

            List<string> values = new List<string>();
            foreach (var node in pt.Nodes)
            {
                TableName tname = (TableName)node.Item;
                ++i;

                if (IsMatch(cmd.Wildcard, tname.Path) || (tname.SchemaName == SchemaName.dbo && IsMatch(cmd.Wildcard, tname.Name)))
                {
                    string desc = "TABLE";
                    switch (tname.Type)
                    {
                        case TableNameType.Table:
                            desc = "TABLE";
                            count[0]++;
                            break;

                        case TableNameType.View:
                            desc = "VIEW";
                            count[1]++;
                            break;

                        case TableNameType.Procedure:
                            desc = "PROC";
                            count[2]++;
                            break;

                        case TableNameType.Function:
                            desc = "FUNC";
                            count[3]++;
                            break;
                    }


                    cout.WriteLine("{0,5} {1,15}.{2,-37} <{3}>", sub(i), tname.SchemaName, tname.Name, desc);

                    h = PagePause(cmd, ++h);

                    values.Add(tname.ShortName);
                }
            }

            Assign(cmd, values);
            cout.WriteLine("\t{0} Table(s)", count[0]);
            cout.WriteLine("\t{0} View(s)", count[1]);
            cout.WriteLine("\t{0} Procedure(s)", count[2]);
            cout.WriteLine("\t{0} Function(s)", count[3]);

            return true;
        }

        private static int PagePause(ApplicationCommand cmd, int h)
        {
            if (cmd.HasPage && h >= Console.WindowHeight - 1)
            {
                h = 0;
                cout.Write("press any key to continue...");
                cin.ReadKey();
                cout.WriteLine();
            }
            return h;
        }



        private static bool DisplayTableSubNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (!(pt.Item is TableName))
                return false;

            TableName tname = (TableName)pt.Item;

            bool flag = false;

            if (cmd.HasDefinition)
            {
                if (tname.Type == TableNameType.View)
                {
                    cout.WriteLine("cannot display view structure");
                    return false;
                }

                _DisplayColumnNodes(cmd, tname);
                flag = true;
            }

            if (cmd.HasPrimaryKey)
            {
                displayTable(tname.PrimaryKeySchema(), "Primary Key(s)");
                flag = true;
            }

            if (cmd.HasForeignKey)
            {
                displayTable(tname.ForeignKeySchema(), "Foreign Key(s)");
                flag = true;
            }

            if (cmd.HasDependency)
            {
                displayTable(tname.DependenySchema(), "Dependencies");
                flag = true;
            }

            if (cmd.HasIdentityKey)
            {
                displayTable(tname.IdentityKeySchema(), "Identity Key(s)");
                flag = true;
            }

            if (cmd.HasIndex)
            {
                displayTable(tname.IndexSchema(), "Indices");
                flag = true;
            }


            if (cmd.HasStorage)
            {
                displayTable(tname.StorageSchema(), "Storage");
                flag = true;
            }


            if (flag)
                return true;

            _DisplayLocatorNodes(pt, cmd);
            return true;
        }

        private static void displayTable(DataTable dt, string title)
        {
            if (dt.Rows.Count > 0)
            {
                cout.WriteLine("<{0}>", title);
                dt.ToConsole();
            }
            else
            {
                cout.WriteLine(title + " not found");
            }
        }


        private static void _DisplayColumnNodes(ApplicationCommand cmd, TableName tname)
        {
            TableSchema schema = new TableSchema(tname);
            cout.WriteLine("TABLE: {0}", tname.Path);

            bool hasJson = cmd.Has("json");
            VAL lines = new VAL();

            int i = 0;
            int count = 0;
            int h = 0;
            foreach (IColumn column in schema.Columns)
            {
                if (IsMatch(cmd.Wildcard, column.ColumnName))
                {
                    count++;

                    List<string> L = new List<string>();
                    if (column.IsIdentity) L.Add("++");
                    if (column.IsPrimary) L.Add("pk");
                    if ((column as ColumnSchema).IsForeignKey) L.Add("fk");
                    string keys = string.Join(",", L);

                    if (!hasJson)
                    {
                        cout.WriteLine("{0,5} {1,26} {2,-16} {3,10} {4,10}",
                           sub(++i),
                           string.Format("[{0}]", column.ColumnName),
                           column.GetSQLType(),
                           keys,
                           column.Nullable ? "null" : "not null");
                    }
                    else
                    {
                        VAL line = new VAL();
                        if (keys != "")
                            line.AddMember("Key", keys);
                        line.AddMember("Column", column.ColumnName);
                        line.AddMember("Type", column.GetSQLType());
                        if (column.Nullable)
                            line.AddMember("Null", column.Nullable);
                        lines.Add(line);
                    }

                    h = PagePause(cmd, ++h);
                }
            }

            if (!hasJson)
                cout.WriteLine("\t{0} Column(s)", count);
            else
                cout.WriteLine(lines.ToJson());
        }

        private static bool _DisplayLocatorNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            int i = 0;
            int count = 0;
            foreach (var node in pt.Nodes)
            {
                IDataPath item = node.Item;
                ++i;

                if (IsMatch(cmd.Wildcard, item.Path))
                {
                    count++;

                    if (item is Locator)
                    {
                        Locator locator = (Locator)item;
                        if (!string.IsNullOrEmpty(locator.Name))
                        {
                            cout.WriteLine("{0,5} {1,-20} {2}", sub(i), locator.Name, locator.Where);
                            continue;
                        }
                    }

                    cout.WriteLine("{0,5} {1}", sub(i), item);
                }
            }

            cout.WriteLine("\t{0} Item(s)", count);
            return true;
        }

        private static bool DisplayViewNodes(TreeNode<IDataPath> pt, ApplicationCommand cmd)
        {
            if (!(pt.Item is TableName))
                return false;

            TableName vname = (TableName)pt.Item;
            if (vname.Type != TableNameType.View)
                return false;

            DataTable schema = vname.ViewSchema();
            cout.WriteLine("VIEW: {0}", vname.Path);

            int i = 0;
            int count = 0;
            int h = 0;
            foreach (DataRow row in schema.Rows)
            {
                string columnName = string.Format("{0}", row["COLUMN_NAME"]);
                if (IsMatch(cmd.Wildcard, columnName))
                {
                    count++;

                    cout.WriteLine("{0,5} {1,26} {2,-16} {3,10}",
                        sub(++i),
                        string.Format("{0}", columnName),
                        row["DATA_TYPE"],
                        (string)row["IS_NULLABLE"] == "YES" ? "null" : "not null");

                    h = PagePause(cmd, ++h);
                }

            }
            cout.WriteLine("\t{0} Column(s)", count);

            return true;
        }


        private static string sub(int i)
        {
            return string.Format("[{0}]", i);
        }
    }
}
