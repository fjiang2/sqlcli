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
			: base(new SqlDbAgent(provider))
		{
			Description = provider.ToString();
		}


	}
}
