using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sys.Data;
using Sys.Data.Linq;

namespace UnitTestProject
{
	class MyDataContext : DataContext
	{
		public MyDataContext()
			: this(ConnectionProviderManager.DefaultProvider)
		{
		}

		public MyDataContext(string connectionString)
			: this(ConnectionProvider.CreateProvider("ServerName", connectionString))
		{
		}

		public MyDataContext(ConnectionProvider provider)
			: base(query => new SqlCmd(provider, query))
		{
			Description = provider.ToString();
		}


	}
}
