using System;
using System.Collections.Generic;
using System.Linq;
using Sys.Data;
using Sys.Stdio;
using Sys;

namespace sqlcli
{
    class ShellContext
    {
        public ISide TheSide { get; set; }

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
            this.commandee = new Commandee(mgr);

            string server = connection.Home;

            ConnectionProvider pvd = null;
            if (!string.IsNullOrEmpty(server))
                pvd = connection.GetProvider(server);

            if (pvd != null)
            {
                TheSide = new Side(pvd);
                ChangeSide(TheSide);
            }
            else if (connection.Providers.Count() > 0)
            {
                TheSide = new Side(connection.Providers.First());
                ChangeSide(TheSide);
            }
            else
            {
                cerr.WriteLine("database server not defined");
            }
        }

        public void ChangeSide(ISide side)
        {
            if (side == null)
            {
                cerr.WriteLine("undefined side");
                return;
            }

            this.TheSide = side;
            Context.DS.AddHostObject(THESIDE, side);

            commandee.chdir(TheSide.Provider.ServerName, TheSide.DatabaseName);
        }

    }
}
