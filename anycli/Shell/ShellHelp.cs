using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using syscon.stdio;

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
            Cout.WriteLine("Path points to server, database,tables, data rows");
            Cout.WriteLine(@"      \server\database\table\filter\filter\....");
            Cout.WriteLine("Notes: table names support wildcard matching, e.g. Prod*,Pro?ucts");
            Cout.WriteLine("exit                    : quit application");
            Cout.WriteLine("help                    : this help");
            Cout.WriteLine("?                       : this help");
            Cout.WriteLine("rem                     : comments or remarks");
            Cout.WriteLine("ver                     : display version");
            Cout.WriteLine("cls                     : clears the screen");
            Cout.WriteLine("echo /?                 : display text");
            Cout.WriteLine();
        }
    }
}
