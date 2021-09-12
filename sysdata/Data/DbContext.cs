using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sys.Data.Entity;

namespace Sys.Data
{
	public class DbContext : DataContext
	{
		public DbContext()
			: this(ConnectionProviderManager.DefaultProvider)
		{
		}

		public DbContext(string connectionString)
			: this(ConnectionProvider.CreateProvider("ServerName", connectionString))
		{
		}

		public DbContext(ConnectionProvider provider)
			: base(DbAgent.Create(DbAgentStyle.SqlServer, (query, args) => new SqlCmd(provider, query, args)))
		{
			Description = provider.ToString();
		}


	}
}
