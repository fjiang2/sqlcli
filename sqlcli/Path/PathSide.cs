﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data;
using Sys;
using Sys.Stdio;
using Sys.Stdio.Cli;

namespace sqlcli
{
    class PathSide
    {
        public Side side
        {
            get; private set;
        }

        private DatabaseName dname;
        private TreeNode<IDataPath> node;
        private TableName[] T;   //wildcard matched tables

        private readonly PathManager mgr;
        private readonly ApplicationCommand cmd;

        public PathSide(PathManager mgr, ApplicationCommand cmd)
        {
            this.mgr = mgr;
            this.cmd = cmd;
        }

        public TableName[] MatchedTables
        {
            get { return this.T; }
        }

        public TreeNode<IDataPath> Node
        {
            get { return this.node; }
        }

        public bool SetSource(string source)
        {
            return SetSource(source, "source");
        }

        private bool SetSource(string source, string sourceText)
        {
            if (source == null)
            {
                cerr.WriteLine("invalid argument");
                return false;
            }

            var path = new PathName(source);

            node = mgr.Navigate(path);
            if (node == null)
            {
                cerr.WriteLine("invalid path:" + path);
                return false;
            }

            dname = mgr.GetPathFrom<DatabaseName>(node);
            if (dname == null)
            {
                cerr.WriteLine($"warning: {sourceText} database is unavailable");
                return false;
            }

            var server = mgr.GetPathFrom<ServerName>(node);
            side = new Side(dname);

            T = new TableName[] { };

            if (path.Wildcard != null)
            {
                var m1 = new MatchedDatabase(dname, path.Wildcard)
                {
                    Includedtables = cmd.Includes,
                    Excludedtables = cmd.Excludes
                };

                T = m1.TableNames();
            }
            else
            {
                TableName tname = mgr.GetPathFrom<TableName>(node);
                if (tname == null)
                {
                    T = dname.GetTableNames();
                }
                else
                {
                    T = new TableName[] { tname };
                }
            }

            return true;
        }

        public bool SetSink(string sink)
        {
            if (sink != null)
            {
                return SetSource(sink, "destination");
            }
            else
                node = mgr.current;

            dname = mgr.GetPathFrom<DatabaseName>(node);
            if (dname == null)
            {
                cerr.WriteLine("warning: destination database is unavailable");
                return false;
            }

            var tname = mgr.GetPathFrom<TableName>(node);
            if (tname != null)
                T = new TableName[] { tname };
            else
                T = dname.GetTableNames();

            var server = mgr.GetPathFrom<ServerName>(node);
            side = new Side(dname);

            return true;
        }

    }
}
