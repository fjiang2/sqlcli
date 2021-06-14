using System;
using System.Data;
using System.Linq;
using Sys.Data;
using Sys.Data.Comparison;
using Tie;
using Sys.Stdio;
using Sys.Stdio.Cli;

namespace sqlcli
{
    class ShellTask : ShellContext, IShellTask
    {
        public ShellTask(IApplicationConfiguration cfg)
            : base(cfg)
        {
        }

        public void Help()
        {
            ShellHelp.Help();
        }

        public IShellTask CreateTask()
        {
            return new ShellTask(cfg);
        }

        public string CurrentPath => mgr.ToString();

        public NextStep DoSingleLineCommand(string line)
        {
            line = line.Trim();
            if (line == string.Empty)
                return NextStep.CONTINUE;

            ApplicationCommand cmd = new ApplicationCommand(cfg, line);
            if (cmd.InvalidCommand)
                return NextStep.ERROR;

            switch (cmd.Action)
            {
                case "set":
                    commandee.set(cmd);
                    return NextStep.COMPLETED;

                case "let":
                    commandee.let(cmd);
                    return NextStep.COMPLETED;

                case "md":
                case "mkdir":
                    commandee.mkdir(cmd);
                    return NextStep.COMPLETED;

                case "rd":
                case "rmdir":
                    commandee.rmdir(cmd);
                    return NextStep.COMPLETED;
            }


            switch (cmd.Action)
            {
                case "ls":
                case "dir":
                    commandee.dir(cmd);
                    return NextStep.COMPLETED;

                case "cd":
                case "chdir":
                    if (cmd.Arg1 != null || cmd.HasHelp)
                        chdir(cmd);
                    else
                        cout.WriteLine(mgr.ToString());
                    return NextStep.COMPLETED;

                case "type":
                    commandee.type(cmd);
                    return NextStep.COMPLETED;

                case "del":
                case "erase":
                    commandee.del(cmd);
                    return NextStep.COMPLETED;

                case "ren":
                case "rename":
                    commandee.rename(cmd);
                    return NextStep.COMPLETED;

                case "attrib":
                    commandee.attrib(cmd);
                    return NextStep.COMPLETED;

                case "echo":
                    commandee.echo(cmd);
                    return NextStep.COMPLETED;

                case "rem":
                    return NextStep.COMPLETED;

                case "ver":
                    cout.WriteLine("sqlcli [Version {0}]", Helper.ApplicationVerison);
                    return NextStep.COMPLETED;

                case "show":
                    if (cmd.Arg1 != null)
                        Show(cmd.Arg1.ToLower(), cmd.Arg2);
                    else
                        cerr.WriteLine("invalid argument");
                    return NextStep.COMPLETED;

                case "find":
                    commandee.find(cmd, cmd.Arg1);
                    return NextStep.COMPLETED;

                case "save":
                    commandee.save(cmd);
                    return NextStep.COMPLETED;

                case "execute":
                    commandee.execute(cmd, theSide);
                    if (commandee.ErrorCode == CommandState.OK)
                        return NextStep.COMPLETED;
                    else
                        return NextStep.ERROR;

                case "open":
                    commandee.open(cmd);
                    return NextStep.COMPLETED;

                case "compare":
                    commandee.compare(cmd);
                    return NextStep.COMPLETED;

                case "copy":
                    commandee.copy(cmd, CompareSideType.copy);
                    return NextStep.COMPLETED;

                case "sync":
                    commandee.copy(cmd, CompareSideType.sync);
                    return NextStep.COMPLETED;

                case "comp":
                    commandee.copy(cmd, CompareSideType.compare);
                    return NextStep.COMPLETED;

                case "xcopy":
                    commandee.xcopy(cmd);
                    return NextStep.COMPLETED;

                case "lcd":
                    if (cmd.Arg1 != null)
                        cfg.WorkingDirectory.ChangeDirectory(cmd.Arg1);
                    else
                        cout.WriteLine(cfg.WorkingDirectory.CurrentDirectory);
                    return NextStep.COMPLETED;

                case "ldir":
                    cfg.WorkingDirectory.ShowCurrentDirectory(cmd.Arg1);
                    return NextStep.COMPLETED;

                case "ltype":
                    if (cmd.Arg1 != null)
                    {
                        string[] lines = cfg.WorkingDirectory.ReadAllLines(cmd.Arg1);
                        if (lines != null)
                        {
                            foreach (var _line in lines)
                                cout.WriteLine(_line);
                        }
                    }
                    else
                        cout.WriteLine("invalid arguments");
                    return NextStep.COMPLETED;

                case "path":
                    if (cmd.Arg1 == null)
                        cout.WriteLine(cfg.Path);
                    else
                        Context.SetValue("path", cmd.Arg1);
                    return NextStep.COMPLETED;

                case "run":
                    if (cmd.Arg1 != null)
                        Shell.RunBatch(this, cfg, cmd.Arg1, cmd.Arguments);
                    else
                        cout.WriteLine("invalid arguments");
                    return NextStep.COMPLETED;

                case "call":
                    if (!commandee.call(cmd))
                        return NextStep.ERROR;
                    else
                        return NextStep.COMPLETED;

                case "import":
                    commandee.import(cmd);
                    return NextStep.COMPLETED;

                case "export":
                    commandee.export(cmd);
                    return NextStep.COMPLETED;

                case "load":
                    commandee.load(cmd);
                    return NextStep.COMPLETED;

                case "clean":
                    commandee.clean(cmd);
                    return NextStep.COMPLETED;

                case "mount":
                    commandee.mount(cmd, connection);
                    return NextStep.COMPLETED;

                case "umount":
                    commandee.umount(cmd, connection);
                    return NextStep.COMPLETED;

                case "edit":
                    commandee.edit(cmd, connection, theSide);
                    return NextStep.COMPLETED;

                case "last":
                    commandee.last(cmd);
                    return NextStep.COMPLETED;

                case "chk":
                case "check":
                    commandee.check(cmd);
                    return NextStep.COMPLETED;

                default:
                    if (!_SQL.Contains(cmd.Action.ToUpper()))
                    {
                        cerr.WriteLine("invalid command");
                        return NextStep.COMPLETED;
                    }
                    break;
            }

            return NextStep.NEXT;
        }

        static readonly string[] _SQL = new string[] { "ALTER", "CREATE", "DELETE", "DROP", "EXEC", "INSERT", "SELECT", "UPDATE", "USE" };


        private void chdir(ApplicationCommand cmd)
        {
            if (commandee.chdir(cmd))
            {
                var dname = mgr.GetCurrentPath<DatabaseName>();
                if (dname != null)
                {
                    if (theSide == null)
                        theSide = new Side(dname);
                    else
                        theSide.UpdateDatabase(dname);
                }
                else
                {
                    var sname = mgr.GetCurrentPath<ServerName>();
                    if (sname != null)
                    {
                        if (theSide == null)
                            theSide = new Side(dname);
                        else
                            theSide.UpdateDatabase(sname.Provider);
                    }
                }
            }
        }

        private static string showConnection(ConnectionProvider cs)
        {
            return string.Format("S={0} db={1} U={2} P={3}", cs.DataSource, cs.InitialCatalog, cs.UserId, cs.Password);
        }

        public NextStep DoMultipleLineCommand(string text)
        {
            text = text.Trim();
            if (text == string.Empty)
                return NextStep.NEXT;

            string[] A = text.Split(' ', '\r');
            string cmd = null;
            string arg1 = null;
            string arg2 = null;

            int n = A.Length;

            if (n > 0)
                cmd = A[0].ToLower();

            if (n > 1)
                arg1 = A[1].Trim();

            if (n > 2)
                arg2 = A[2].Trim();

            switch (cmd)
            {
                case "use":
                case "select":
                    if (!Context.GetValue<bool>(Context.DATAREADER))
                    {
                        DataSet ds = new SqlCmd(theSide.Provider, text).FillDataSet();
                        if (ds != null)
                        {
                            foreach (DataTable dt in ds.Tables)
                                dt.ToConsole();
                        }
                    }
                    else
                    {
                        new SqlCmd(theSide.Provider, text).Read(reader => reader.ToConsole(cfg.MaxRows));
                    }
                    break;

                case "update":
                case "delete":
                case "insert":
                case "exec":
                case "create":
                case "alter":
                case "drop":
                    try
                    {
                        int count = new SqlCmd(theSide.Provider, text).ExecuteNonQuery();
                        if (count > 0)
                            cout.WriteLine("{0} of row(s) affected", count);
                        else if (count == 0)
                            cout.WriteLine("nothing affected");
                        else
                            cout.WriteLine("command(s) completed successfully");
                    }
                    catch (Exception ex)
                    {
                        cerr.WriteLine(ex.Message);
                        return NextStep.ERROR;
                    }
                    break;

                default:
                    cerr.WriteLine("invalid command");
                    break;
            }

            return NextStep.COMPLETED;
        }

        private void Show(string arg1, string arg2)
        {
            var dname = theSide.DatabaseName;
            TableName[] vnames;

            switch (arg1)
            {
                case "pk":
                    {
                        var PKS = dname.TableWithPrimaryKey();
                        int count = 0;
                        foreach (var tname in PKS)
                        {
                            count++;
                            cout.WriteLine("{0,5} {1}", $"[{count}]", tname);
                        }
                        cout.WriteLine("total <{0}> tables with primary keys", count);
                    }
                    break;

                case "npk":
                    {
                        var tnames = dname.GetTableNames();
                        var PKS = dname.TableWithPrimaryKey();
                        int count = 0;
                        foreach (var tname in tnames)
                        {
                            if (PKS.FirstOrDefault(row => row.Equals(tname)) == null)
                            {
                                count++;
                                cout.WriteLine("{0,5} {1}", $"[{count}]", tname);
                            }
                        }
                        cout.WriteLine("total <{0}> tables without primary keys", count);
                    }
                    break;

                case "vw":
                    vnames = new MatchedDatabase(dname, arg2).ViewNames();
                    foreach (var vname in vnames)
                    {
                        DataTable dt = null;
                        dt = vname.ViewSchema();
                        if (dt.Rows.Count > 0)
                        {
                            cout.WriteLine("<{0}>", vname.ShortName);
                            dt.ToConsole();
                        }
                        else
                            cout.WriteLine("not found at <{0}>", vname.ShortName);
                    }
                    break;

                case "view":
                    vnames = new MatchedDatabase(dname, arg2).ViewNames();
                    vnames.Select(tname => new { Schema = tname.SchemaName, View = tname.Name })
                        .ToConsole();
                    break;

                case "proc":
                    dname.AllProc().ToConsole();
                    break;

                case "index":
                    dname.AllIndices().ToConsole();
                    break;

                case "connection":
                    {
                        var L = connection.Providers.OrderBy(x => x.ServerName.Path);
                        if (L.Any())
                        {
                            L.Select(pvd => new { Alias = pvd.ServerName.Path, Connection = pvd.ToSimpleString() })
                            .ToConsole();
                        }
                        else
                            cerr.WriteLine("connection string not found");
                    }
                    break;

                case "current":
                    cout.WriteLine("current: {0}({1})", theSide.Provider.Name, showConnection(theSide.Provider));
                    break;

                case "var":
                    {
                        ((VAL)Context.DS)
                            .Where(row => row[1].VALTYPE != VALTYPE.nullcon && row[1].VALTYPE != VALTYPE.voidcon && !row[0].Str.StartsWith("$"))
                            .Select(row => new { Variable = (string)row[0], Value = row[1] })
                            .ToConsole();
                    }
                    break;
                default:
                    cerr.WriteLine("invalid argument");
                    break;
            }
        }



    }
}
