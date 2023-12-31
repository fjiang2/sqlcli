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
			Cerr.WriteLine("invalid arguments");
		}

		public void echo(ApplicationCommand cmd)
		{
			if (cmd.HasHelp)
			{
				Cout.WriteLine("Displays messages, or turns command-echoing on or off");
				Cout.WriteLine("  echo [on | off]");
				Cout.WriteLine("  echo [message]");
				Cout.WriteLine("Type echo without parameters to display the current echo setting.");
				return;
			}

			string text = cmd.Args;
			if (string.IsNullOrEmpty(text))
			{
				string status = "on";
				if (!Cout.Echo)
					status = "off";

				Cout.WriteLine($"echo is {status}");
				return;
			}

			switch (text)
			{
				case "on":
					Cout.Echo = true;
					break;

				case "off":
					Cout.Echo = false;
					break;

				default:
					Cout.WriteLine(text);
					break;
			}

			return;
		}





		public void find(ApplicationCommand cmd, string match)
		{
			if (cmd.HasHelp)
			{
				Cout.WriteLine("find command searches name of database, schema, table, view, or column");
				Cout.WriteLine("example:");
				Cout.WriteLine("  find *ID*           : search any string contains ID");
				Cout.WriteLine("  find *na?e          : search string ends with na?e");
				return;
			}


		}

		public bool call(ApplicationCommand cmd)
		{

			return true; // NextStep.COMPLETED;
		}

	}
}

