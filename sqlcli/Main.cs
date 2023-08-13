using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Sys;
using syscon.stdio;
using syscon.stdio.Cli;
using Sys.Data;

namespace sqlcli
{
    class Main
    {

        private readonly ApplicationConfiguration cfg;
        private IShell shell;

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
                            Cout.WriteLine("/i undefined sql script file name");
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
                            Cout.WriteLine("/o undefined sql script file name");
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
            shell = new Shell(task);
            Context.DS.AddHostObject(Context.SHELL, shell);
            shell.Run();
        }

        public static void ShowHelp()
        {
            Cout.WriteLine("SQL Server Command Line Interface");
            Cout.WriteLine("Usage: sqlcli");
            Cout.WriteLine("     [/cfg configuration file name (.cfg)]");
            Cout.WriteLine("     [/i sql script file name (.sql)]");
            Cout.WriteLine("     [file] sqlcli command batch file name (.sqc)");
            Cout.WriteLine();
            Cout.WriteLine("/h,/?      : this help");
            Cout.WriteLine($"/cfg       : congfiguration file default file: \"{ConfigurationEnvironment.Path.Personal}\"");
            Cout.WriteLine("/i         : input sql script file name");
            Cout.WriteLine("/o         : result of sql script");
            Cout.WriteLine("examples:");
            Cout.WriteLine("  sqlcli file1.sqc");
            Cout.WriteLine("  sqlcli /cfg my.cfg");
            Cout.WriteLine("  sqlcli /i script1.sql /o c:\\temp\\o.txt");
        }
    }
}
