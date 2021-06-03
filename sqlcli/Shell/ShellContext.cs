using System;
using System.Collections.Generic;
using System.Linq;
using Sys.Data;
using Sys.Stdio;
using Sys.Stdio.Cli;

namespace sqlcli
{
    class ShellContext
    {
        protected Side theSide { get; set; }
        protected PathManager mgr { get; }
        protected IApplicationConfiguration cfg { get; }
        protected IConnectionConfiguration connection { get; }
        protected Commandee commandee { get; }
        protected const string THESIDE = "$TheSide";

        public ShellContext(IApplicationConfiguration cfg)
        {
            this.cfg = cfg;
            this.connection = cfg.Connection;
            this.mgr = new PathManager(connection);
            this.commandee = new Commandee(mgr, cfg);

            string server = connection.Home;

            ConnectionProvider pvd = null;
            if (!string.IsNullOrEmpty(server))
                pvd = connection.GetProvider(server);

            if (pvd != null)
            {
                theSide = new Side(pvd);
                ChangeSide(theSide);
            }
            else if (connection.Providers.Count > 0)
            {
                theSide = new Side(connection.Providers.First());
                ChangeSide(theSide);
            }
            else
            {
                cerr.WriteLine("database server not defined");
            }
        }

        protected void ChangeSide(Side side)
        {
            if (side == null)
            {
                cerr.WriteLine("undefined side");
                return;
            }

            this.theSide = side;
            Context.DS.AddHostObject(THESIDE, side);

            commandee.chdir(theSide.Provider.ServerName, theSide.DatabaseName);
        }

        public void SwitchTask(IShellTask context)
        {
            ChangeSide((context as ShellContext).theSide);
        }
    }
}
