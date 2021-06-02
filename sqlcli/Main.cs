using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Sys;
using Sys.Stdio;
using Sys.Cli;
using Sys.Data;

namespace sqlcli
{
    class Main
    {

        private ApplicationConfiguration cfg;
        public Shell Shell { get; private set; }

        public Main(ApplicationConfiguration cfg)
        {
            this.cfg = cfg;
        }


        public void Run(string[] args)
        {
            int i = 0;

            while (i < args.Length)
            {
                string arg = args[i++];
                switch (arg)
                {
                    case "/cfg":
                        i++;
                        break;


                    case "/i":
                        if (i < args.Length && !args[i].StartsWith("/"))
                        {
                            IConnectionConfiguration connection = cfg.Connection;
                            string inputfile = args[i++];
                            string server = connection.Home;
                            var pvd = connection.GetProvider(server);
                            var theSide = new Side(pvd);
                            theSide.ExecuteScript(inputfile, verbose: false);
                            break;
                        }
                        else
                        {
                            cout.WriteLine("/i undefined sql script file name");
                            return;
                        }

                    case "/o":
                        if (i < args.Length && !args[i].StartsWith("/"))
                        {
                            cfg.OutputFile = args[i++];
                            break;
                        }
                        else
                        {
                            cout.WriteLine("/o undefined sql script file name");
                            return;
                        }

                    default:
                        if (!string.IsNullOrEmpty(arg))
                            Shell.RunBatch(cfg, arg, args);
                        else
                            ShowHelp();

                        return;
                }
            }

            ShellTask task = new ShellTask(cfg);
            Shell = new Shell(task);
            Context.DS.AddHostObject(Context.SHELL, Shell);
            Shell.Run();
        }

        public static void ShowHelp()
        {
            cout.WriteLine("SQL Server Command Console");
            cout.WriteLine("Usage: sqlcli");
            cout.WriteLine("     [/cfg configuration file name (.cfg)]");
            cout.WriteLine("     [/i sql script file name (.sql)]");
            cout.WriteLine("     [file] sqlcli command batch file name (.sqc)");
            cout.WriteLine();
            cout.WriteLine("/h,/?      : this help");
            cout.WriteLine($"/cfg       : congfiguration file default file: \"{ConfigurationEnvironment.Path.Personal}\"");
            cout.WriteLine("/i         : input sql script file name");
            cout.WriteLine("/o         : result of sql script");
            cout.WriteLine("examples:");
            cout.WriteLine("  sqlcli file1.sqc");
            cout.WriteLine("  sqlcli /cfg my.cfg");
            cout.WriteLine("  sqlcli /i script1.sql /o c:\\temp\\o.txt");
        }
    }
}
