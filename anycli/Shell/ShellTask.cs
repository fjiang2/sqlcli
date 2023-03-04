using System;
using System.Data;
using System.Linq;
using Sys.Data;
using Tie;
using syscon.stdio;
using syscon.stdio.Cli;

namespace anycli
{
	class ShellTask : ShellContext, IShellTask
	{
		public ShellTask(IApplicationConfiguration cfg)
			: base(cfg)
		{
		}

		public void Help()
		{
			ShellHelp.Help();
		}

		public IShellTask CreateTask()
		{
			return new ShellTask(cfg);
		}


		public NextStep DoSingleLineCommand(string line)
		{
			line = line.Trim();
			if (line == string.Empty)
				return NextStep.CONTINUE;

			ApplicationCommand cmd = new ApplicationCommand(cfg, line);
			if (cmd.InvalidCommand)
				return NextStep.ERROR;

			switch (cmd.Action)
			{
				case "let":
					return NextStep.COMPLETED;

				case "type":
					return NextStep.COMPLETED;


				case "echo":
					commandee.echo(cmd);
					return NextStep.COMPLETED;

				case "rem":
					return NextStep.COMPLETED;

				case "ver":
					cout.WriteLine("anycli [Version {0}]", ShellHelp.ApplicationVerison);
					return NextStep.COMPLETED;

				case "path":
					if (cmd.Arg1 == null)
						cout.WriteLine(cfg.Path);
					else
						Context.SetValue("path", cmd.Arg1);
					return NextStep.COMPLETED;

				case "run":
					if (cmd.Arg1 != null)
						Shell.RunBatch(this, cfg, cmd.Arg1, cmd.Arguments);
					else
						cout.WriteLine("invalid arguments");
					return NextStep.COMPLETED;

				case "call":
					if (!commandee.call(cmd))
						return NextStep.ERROR;
					break;

				default:
					cerr.WriteLine("invalid command");
					break;
			}

			return NextStep.NEXT;
		}


		string IShellTask.CurrentPath => "$";

		public NextStep DoMultipleLineCommand(string text)
		{
			text = text.Trim();
			if (text == string.Empty)
				return NextStep.NEXT;

			string[] A = text.Split(' ', '\r');
			string cmd = null;
			string arg1 = null;
			string arg2 = null;

			int n = A.Length;

			if (n > 0)
				cmd = A[0].ToLower();

			if (n > 1)
				arg1 = A[1].Trim();

			if (n > 2)
				arg2 = A[2].Trim();

			switch (cmd)
			{
				case "exec":
					try
					{
						cout.WriteLine("command(s) completed successfully");
					}
					catch (Exception ex)
					{
						cerr.WriteLine(ex.Message);
						return NextStep.ERROR;
					}
					break;

				default:
					cerr.WriteLine("invalid command");
					break;
			}

			return NextStep.COMPLETED;
		}


	}
}
