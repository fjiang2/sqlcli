using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using syscon.stdio;
using syscon.grid;

namespace sqlcli
{
    static class ConsoleGrid
    {
      

        public static void ToConsole(this DataTable dt, bool vertical = false, bool more = false, bool outputDbNull = true, int maxColumnWidth = 0)
        {
            ShellHistory.SetLastResult(dt);
            OutputDataTable odt = new OutputDataTable(dt, Cout.TrimWriteLine, vertical)
            {
                OutputDbNull = outputDbNull,
                MaxColumnWidth = maxColumnWidth,
            };
            odt.Output();

            Cout.WriteLine("<{0}{1} row{2}>", more ? "top " : "", dt.Rows.Count, dt.Rows.Count > 1 ? "s" : "");
        }



    }


}
