using System;
using System.Collections.Generic;
using System.Linq;
using Sys.Data;
using Sys.Stdio;
using Sys.Stdio.Cli;

namespace anycli
{
    class ShellContext
    {
        protected IApplicationConfiguration cfg { get; }
        protected Commandee commandee { get; }

        public ShellContext(IApplicationConfiguration cfg)
        {
            this.cfg = cfg;
            this.commandee = new Commandee(cfg);
        }

        public void SwitchTask(IShellTask context)
        {
        }
    }
}
