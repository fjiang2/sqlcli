using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Sys;
using syscon.stdio;
using syscon.stdio.Cli;

namespace anycli
{
    class ApplicationCommand : Command, IApplicationCommand
    {
        public bool HasHelp { get; private set; }
        public int Top { get; private set; }
        private readonly IApplicationConfiguration cfg;

        public ApplicationCommand(IApplicationConfiguration cfg, string line)
        {
            this.cfg = cfg;

            ParseLine(line);
        }

        protected override bool CustomerizeOption(string a)
        {
            switch (a)
            {
                case "/?":
                    HasHelp = true;
                    return true;

                case "/sto":
                    return true;

                default:
                    if (a.StartsWith("/top:"))
                    {
                        if (int.TryParse(a.Substring(5), out var _top))
                            Top = _top;

                        return true;
                    }

                    return false;
            }
        }

        public IApplicationConfiguration Configuration => this.cfg;


        public string Wildcard
        {
            get
            {
                foreach (var path in paths)
                {
                    var pathName = new PathName(path);

                    if (pathName.Wildcard != null)
                        return pathName.Wildcard;
                }

                if (Path1 == null)
                    return null;
                else
                    return Path1.Wildcard;
            }
        }

      

        public string OutputPath()
        {
            
            string path = GetValue("out");
            if (path == null)
                return null;

            return path;
        }

  

        public string InputPath()
        {
            string path = GetValue("in");
            if (path == null)
                return null;

            return path;
        }

       
        public string GetValue(string name, string configKey, string defaultValue)
        {
            return GetValue(name) ?? cfg.GetValue<string>(configKey, defaultValue);
        }
    }
}
