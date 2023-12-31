using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syscon.stdio
{
    public class Cerr
    {
        public static void WriteLine(string text)
        {
            var keep = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.Error.WriteLine(text);
            Clog.WriteLine(text);

            Console.ForegroundColor = keep;
        }

        public static void WriteLine(string text, Exception ex)
        {
            WriteLine($"{text}, {ex.Message}");
        }
    }
}
