using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Stdio;

namespace anycli
{
    partial class ShellHelp
    {
        public static Version ApplicationVerison
        {
            get
            {
                return System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            }
        }

        public static void Help()
        {
            cout.WriteLine("Path points to server, database,tables, data rows");
            cout.WriteLine(@"      \server\database\table\filter\filter\....");
            cout.WriteLine("Notes: table names support wildcard matching, e.g. Prod*,Pro?ucts");
            cout.WriteLine("exit                    : quit application");
            cout.WriteLine("help                    : this help");
            cout.WriteLine("?                       : this help");
            cout.WriteLine("rem                     : comments or remarks");
            cout.WriteLine("ver                     : display version");
            cout.WriteLine("cls                     : clears the screen");
            cout.WriteLine("echo /?                 : display text");
            cout.WriteLine();
        }
    }
}
