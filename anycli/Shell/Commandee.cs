using Sys;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using syscon.stdio;
using syscon.stdio.Cli;
using Tie;


namespace anycli
{

	internal class Commandee 
	{
		private readonly IApplicationConfiguration cfg;

		public Commandee(IApplicationConfiguration cfg)
		{
			this.cfg = cfg;
		}


		public void save(ApplicationCommand cmd)
		{
			cerr.WriteLine("invalid arguments");
		}

		public void echo(ApplicationCommand cmd)
		{
			if (cmd.HasHelp)
			{
				cout.WriteLine("Displays messages, or turns command-echoing on or off");
				cout.WriteLine("  echo [on | off]");
				cout.WriteLine("  echo [message]");
				cout.WriteLine("Type echo without parameters to display the current echo setting.");
				return;
			}

			string text = cmd.Args;
			if (string.IsNullOrEmpty(text))
			{
				string status = "on";
				if (!cout.echo)
					status = "off";

				cout.WriteLine($"echo is {status}");
				return;
			}

			switch (text)
			{
				case "on":
					cout.echo = true;
					break;

				case "off":
					cout.echo = false;
					break;

				default:
					cout.WriteLine(text);
					break;
			}

			return;
		}





		public void find(ApplicationCommand cmd, string match)
		{
			if (cmd.HasHelp)
			{
				cout.WriteLine("find command searches name of database, schema, table, view, or column");
				cout.WriteLine("example:");
				cout.WriteLine("  find *ID*           : search any string contains ID");
				cout.WriteLine("  find *na?e          : search string ends with na?e");
				return;
			}


		}

		public bool call(ApplicationCommand cmd)
		{

			return true; // NextStep.COMPLETED;
		}

	}
}

