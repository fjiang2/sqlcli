using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys.Data;
using Sys.Data.Comparison;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using syscon.stdio;
using syscon.stdio.Cli;

namespace sqlcli
{
    class Side : IDataPath 
    {
        public DatabaseName DatabaseName { get; private set; }

        public Side(ConnectionProvider provider)
        {
            this.DatabaseName = new DatabaseName(provider, provider.InitialCatalog);
            UpdateDatabase(provider);
        }


        public Side(DatabaseName dname)
        {
            UpdateDatabase(dname);
        }

        public void UpdateDatabase(DatabaseName dname)
        {
            this.DatabaseName = dname;
        }

        public void UpdateDatabase(ConnectionProvider provider)
        {
            this.DatabaseName = new DatabaseName(provider, Provider.InitialCatalog);
        }

        public ConnectionProvider Provider => this.DatabaseName.Provider;

        public string Path => this.Provider.Name;

        public string GenerateScript()
        {
            return DatabaseName.GenerateClause();
        }

        public bool ExecuteScript(string scriptFile, int batchSize = 1, bool verbose = false)
        {
            return ExecuteSqlScript(this.Provider, scriptFile, batchSize, verbose);
        }

        private static bool ExecuteSqlScript(ConnectionProvider provider, string scriptFile, int batchSize, bool verbose)
        {
            if (!File.Exists(scriptFile))
            {
                cerr.WriteLine($"no input file found : {scriptFile}");
                return false;
            }

            cout.WriteLine("executing {0}", scriptFile);
            var script = new SqlScript(provider, scriptFile)
            {
                BatchSize = batchSize
            };

            script.Reported += (sender, e) =>
            {
                if (verbose)
                    cout.WriteLine($"processed line:{e.Line} batch:{e.BatchLine}/{e.BatchSize} total:{e.TotalSize}");
            };

            bool hasError = false;
            script.Error += (sender, e) =>
            {
                hasError = true;
                cerr.WriteLine($"line:{e.Line}, {e.Exception.Message}, SQL:{e.Command}");
            };

            static bool stopOnError()
            {
                return !cin.YesOrNo("are you sure to continue (yes/no)?");
            }

            script.Execute(stopOnError);
            cout.WriteLine("completed.");

            return !hasError;
        }



        public override string ToString()
        {
            return string.Format("Server={0}, Db={1}", Provider.DataSource, this.DatabaseName.Name);
        }

    }
}
