using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace syscon.stdio
{
    public class clog
    {
        private static TextWriter _writer = null;

        public static string LogFileName { get; set; } = "clog.log";

        ~clog()
        {
            if (_writer != null)
                _writer.Close();
        }

        private static TextWriter TexWriter
        {
            get
            {
                if (_writer == null)
                {
                    try
                    {
                        _writer = NewStreamWriter(LogFileName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"failed to create clog file:\"{LogFileName}\", {AllMessages(ex)}");
                       _writer = Console.Error;
                    }
                }

                return _writer;
            }
        }

        private static string AllMessages(Exception exception)
        {
            StringBuilder builder = new StringBuilder(exception.Message);

            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                builder.AppendLine();
                builder.AppendLine(innerException.Message);

                innerException = innerException.InnerException;
            }

            return builder.ToString();
        }


        private static StreamWriter NewStreamWriter(string fileName)
        {
            try
            {
                string folder = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
            }
            catch (ArgumentException)
            {
            }

            return new StreamWriter(fileName);
        }

        public static void Write(string text)
        {
            if (TexWriter == null)
                return;


            TexWriter.Write(text);
            TexWriter.Flush();
        }

        public static void WriteLine(string text)
        {
            if (TexWriter == null)
                return;


            TexWriter.WriteLine(text);
            TexWriter.Flush();
        }
       
    }
}
