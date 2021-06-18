using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sys;
using Sys.IO;
using Sys.Stdio;

namespace Sys.Stdio.Cli
{
    public class Shell : IShell
    {
        public IShellTask Task { get; }
        public Shell(IShellTask task)
        {
            this.Task = task;
        }

        /// <summary>
        /// read command line from console and run command
        /// </summary>
        public void Run()
        {
            string line;

        L1:
            cout.Write($"{Task.CurrentPath}> ");
        L2:
            line = cin.ReadLine();

            if (Console.IsOutputRedirected)
                Console.WriteLine(line);

            //ctrl-c captured
            if (line == null)
                goto L1;

            if (FlowControl.IsFlowStatement(line))
            {
                cerr.WriteLine($"use \"{line}\" on batch script file only");
                goto L1;
            }

            switch (Run(line))
            {
                case NextStep.NEXT:
                case NextStep.COMPLETED:
                case NextStep.ERROR:
                    goto L1;

                case NextStep.CONTINUE:
                    goto L2;

                case NextStep.EXIT:
                    return;

            }
        }

        /// <summary>
        /// process command batch file
        /// </summary>
        /// <param name="lines"></param>
        internal void DoBatch(string[] lines)
        {
            FlowControl flow = new FlowControl(lines);
            NextStep next = flow.Execute(Run);
            if (next == NextStep.EXIT)
                cout.WriteLine(ConsoleColor.Green, "completed.");

            cout.Write($"{Task.CurrentPath}> ");
        }

        private bool multipleLineMode = false;
        private readonly StringBuilder multipleLineBuilder = new StringBuilder();

        public NextStep Run(string line)
        {

            if (!multipleLineMode)
            {

                if (line == "exit")
                    return NextStep.EXIT;

                switch (line)
                {
                    case "help":
                    case "?":
                        Task.Help();
                        multipleLineBuilder.Clear();
                        return NextStep.COMPLETED;

                    case "cls":
                        Console.Clear();
                        return NextStep.COMPLETED;

                    default:
                        {
                            var _result = TrySingleLineCommand(line);
                            if (_result == NextStep.COMPLETED)
                            {
                                cout.WriteLine();
                                return NextStep.COMPLETED;
                            }
                            else if (_result == NextStep.ERROR)
                                return NextStep.ERROR;

                        }
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(line) && line != ";")
                multipleLineBuilder.AppendLine(line);

            if (line.EndsWith(";"))
            {
                string text = multipleLineBuilder.ToString().Trim();
                multipleLineBuilder.Clear();

                if (text.EndsWith(";"))
                    text = text.Substring(0, text.Length - 1);

                try
                {
                    multipleLineMode = false;
                    var result = Task.DoMultipleLineCommand(text);
                    cout.WriteLine();
                    return result;
                }
                catch (Exception ex)
                {
                    cout.WriteLine(ex.AllMessages());
                    return NextStep.ERROR;
                }

            }
            else if (multipleLineBuilder.ToString() != "")
            {
                multipleLineMode = true;
                cout.Write("...");
                return NextStep.CONTINUE;
            }

            return NextStep.NEXT;
        }

        private NextStep TrySingleLineCommand(string line)
        {
#if DEBUG
            return Task.DoSingleLineCommand(line);
#else
            try
            {
                return Task.DoSingleLineCommand(line);
            }
            catch (Exception ex)
            {
                cerr.WriteLine(ex.AllMessages());
            }

            return NextStep.ERROR;
#endif
        }

        public static void RunBatch(IWorkspace workspace, string path, string[] args)
        {
            RunBatch(null, workspace, path, args);
        }

        /// <summary>
        /// Run batch file
        /// </summary>
        /// <param name="task"></param>
        /// <param name="workspace"></param>
        /// <param name="path">batch file name</param>
        /// <param name="args">batch file arguments, arguments is %1,%2,...</param>
        public static void RunBatch(IShellTask task, IWorkspace workspace, string path, string[] args)
        {
            Batch batch = new Batch(workspace, path);
            batch.Call(task, args);
        }
    }
}
