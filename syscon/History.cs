using System;
using System.Collections.Generic;
using System.Text;
using syscon.stdio;

namespace syscon
{
    public static class History
    {
        private static int WindowWidth => Cout.WindowWidth;

        public static SeverityLevel Level { get; set; } = SeverityLevel.Debug;

        public static void Message(string text)
        {
            WriteMessage(SeverityLevel.Unknown, text, null);
        }

        public static void Information(string text)
        {
            WriteMessage(SeverityLevel.Information, text, null);
        }

        public static void Error(string text, Exception exception = null)
        {
            WriteMessage(SeverityLevel.Error, text, exception);
        }

        public static void Debug(string text, Exception exception = null)
        {
            WriteMessage(SeverityLevel.Debug, text, exception);
        }

        public static void Warn(string text, Exception exception = null)
        {
            WriteMessage(SeverityLevel.Warn, text, exception);
        }

        public static void Fatal(string text, Exception exception = null)
        {
            WriteMessage(SeverityLevel.Fatal, text, exception);
        }

        private static void WriteMessage(SeverityLevel level, string text, Exception ex)
        {
            string _level = "";
            if (level != SeverityLevel.Unknown)
            {
                _level = $"<{level}> ";
            }

            if (ex != null)
            {
                text = $"{_level}{text}, {ex.Message}";
            }
            else
            {
                text = $"{_level}{text}";
            }

            ConsoleColor color = ConsoleColor.Gray;
            switch (level)
            {
                case SeverityLevel.Trace:
                    color = ConsoleColor.Blue;
                    break;

                case SeverityLevel.Debug:
                    color = ConsoleColor.White;
                    break;

                case SeverityLevel.Information:
                    color = ConsoleColor.DarkGreen;
                    break;

                case SeverityLevel.Warn:
                    color = ConsoleColor.DarkRed;
                    break;

                case SeverityLevel.Error:
                    color = ConsoleColor.Red;
                    break;

                case SeverityLevel.Fatal:
                    color = ConsoleColor.Yellow;
                    break;
            }

            Clog.WriteLine(text);
            if (level >= Level)
            {
                WriteMessage(color, text);
            }
        }

    
        private static void WriteMessage(ConsoleColor color, string text)
        {
            lock (SyncRoot)
            {
                var old = Console.ForegroundColor;
                Console.ForegroundColor = color;
                WriteMessage(text);
                Console.ForegroundColor = old;
            }
        }

        private static void WriteMessage(string text)
        {
            string message = string.Format("{0} {1}", DateTime.Now, text);
            if (message.Length <= WindowWidth)
                Console.WriteLine(message);
            else
            {
                message = string.Format("{0}", DateTime.Now);
                Console.WriteLine(message);
                var lines = text.Split(new char[] { ';', '\n' });
                foreach (var line in lines)
                    Console.WriteLine("\t\t{0}", line.Trim());
            }
        }

        private static object _syncRoot;
        private static object SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

    }
}
