using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data
{
	public class SqlDbAgent : DbAgent
	{
		public ConnectionProvider provider { get; }

		public SqlDbAgent(ConnectionProvider provider)
		{
			this.provider = provider;
		}

		public override IDbAccess Proxy(SqlUnit unit)
			=> new SqlCmd(provider, unit);

		public override DbAgentOption Option => new DbAgentOption { Style = provider.AgentStyle() };

		public static DataQuery Query(ConnectionProvider provider)
			=> new DataQuery(new SqlDbAgent(provider));
	}
}
