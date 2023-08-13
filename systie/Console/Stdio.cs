﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace syscon.stdio
{
    public sealed class Stdio
    {
        public const string FILE_EDITOR = "editor";
        public const string FILE_LOG = "log";

        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Cout.WriteLine();
            Cout.WriteLine("exit application...");
        }



        public static void OpenEditor(string fileName)
        {
            const string notepad = "notepad.exe";
            if (!File.Exists(fileName))
            {
                Cerr.WriteLine($"cannot find the file: {fileName}");
                return;
            }

            string editor = Context.GetValue<string>(FILE_EDITOR, notepad);
            if (!Launch(fileName, editor))
            {
                if (editor != notepad)
                {
                    //try notepad.exe to open
                    Launch(fileName, notepad);
                }
            }

        }

        private static bool Launch(string fileName, string editor)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.ErrorDialog = true;
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.WorkingDirectory = startin;
            process.StartInfo.FileName = editor;
            process.StartInfo.Arguments = $"\"{fileName}\"";


            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                Cerr.WriteLine($"failed to lauch application: {editor} {fileName}, {ex.Message}");
                return false;
            }

            return true;
        }



    }
}