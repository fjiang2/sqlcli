using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sys;
using Sys.Data;
using Sys.Stdio;

namespace sqlcli
{
    class Shell : IShell
    {
        public IShellTask Task { get; }
        IApplicationConfiguration cfg;
        public Shell(IApplicationConfiguration cfg, IShellTask task)
        {
            this.cfg = cfg;
            this.Task = task;
        }

        /// <summary>
        /// read command line from console and run command
        /// </summary>
        public void DoConsole()
        {

            string line = null;

        L1:
            cout.Write($"{Task.ThePath}> ");
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
        public void DoBatch(string[] lines)
        {
            FlowControl flow = new FlowControl(lines);
            NextStep next = flow.Execute(Run);
            if (next == NextStep.EXIT)
                cout.WriteLine(ConsoleColor.Green, "completed.");

            cout.Write($"{Task.ThePath}> ");
        }

        private bool multipleLineMode = false;
        private StringBuilder multipleLineBuilder = new StringBuilder();

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
                        ShellHelp.Help();
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
                catch (System.Data.SqlClient.SqlException ex1)
                {
                    cerr.WriteLine($"SQL:{ex1.AllMessages()}");
                }
                catch (Exception ex)
                {
                    cout.WriteLine(ex.Message);
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
            line = line.Trim();
            if (line == string.Empty)
                return NextStep.CONTINUE;

            ApplicationCommand cmd = new ApplicationCommand(cfg, line);
            if (cmd.badcommand)
                return NextStep.ERROR;
            
            switch (cmd.Action)
            {
                case "run":
                    if (cmd.arg1 != null)
                    {
                        new Batch(cfg, cmd.arg1).Call(this, cmd.Arguments);
                    }
                    return NextStep.COMPLETED;
            }
#if DEBUG
            return Task.DoSingleLineCommand(cmd);
#else
            try
            {
                return command.DoSingleLineCommand(text);
            }
            catch (System.Data.SqlClient.SqlException ex1)
            {
                cerr.WriteLine($"SQL:{ex1.AllMessages()}");
            }
            catch (Exception ex2)
            {
                cerr.WriteLine(ex2.Message);
            }

            return NextStep.ERROR;
#endif
        }

    }
}
