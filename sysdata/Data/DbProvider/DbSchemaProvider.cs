using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Sys.Data
{
    public abstract class DbSchemaProvider
    {
        protected static string[] __sys_tables = { "master", "model", "msdb", "tempdb" };

        protected ConnectionProvider provider;

        protected DbSchemaProvider(ConnectionProvider provider)
        {
            this.provider = provider;
        }

        public virtual bool Exists(DatabaseName dname)
        {
            return GetDatabaseNames().FirstOrDefault(row => row.Equals(dname)) != null;
        }

        public virtual bool Exists(TableName tname)
        {
            DatabaseName dname = tname.DatabaseName;
            if (!Exists(dname))
                return false;

            return GetTableNames(dname).FirstOrDefault(row => row.Equals(tname)) != null;
        }


        public ServerName ServerName
        {
            get { return this.provider.ServerName; }
        }

        public abstract DatabaseName[] GetDatabaseNames();

        public abstract TableName[] GetTableNames(DatabaseName dname);

        public abstract TableName[] GetViewNames(DatabaseName dname);


        /// <summary>
        /// Get stored procedure names and function names
        /// </summary>
        /// <param name="dname"></param>
        /// <returns></returns>
        public abstract TableName[] GetProcedureNames(DatabaseName dname);

        /// <summary>
        /// Get definition of stored procedure and function
        /// </summary>
        /// <param name="pname"></param>
        /// <returns></returns>
        public abstract string GetProcedure(TableName pname);

        public abstract DataSet GetServerSchema(ServerName sname);
        public abstract DataTable GetDatabaseSchema(DatabaseName dname);
        public abstract DataTable GetTableSchema(TableName tname);
        public abstract DependencyInfo[] GetDependencySchema(DatabaseName dname);

        public static bool IsSystemDatabase(string name)
        {
            return __sys_tables.Contains(name);
        }
    }
}
