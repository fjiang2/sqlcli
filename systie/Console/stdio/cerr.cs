using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Stdio
{
    public class cerr
    {
        public static void WriteLine(string text)
        {
            var keep = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.WriteLine(text);
            clog.WriteLine(text);

            Console.ForegroundColor = keep;
        }

        public static void WriteLine(string text, Exception ex)
        {
            WriteLine($"{text}, {ex.Message}");
        }
    }
}
