using System.Data.SqlClient;
using Sys.Data.Entity;

namespace Sys.Data
{
	public class SqlDbAgent : DbAgent
	{
		public ConnectionProvider provider { get; }

		public SqlDbAgent(ConnectionProvider provider)
			:base(new SqlConnectionStringBuilder(provider.ConnectionString))
		{
			this.provider = provider;
		}

		public override DbAccess Access(SqlUnit unit)
			=> new SqlCmd(provider, unit);

		public override DbAgentOption Option => new DbAgentOption { Style = provider.AgentStyle() };

		public static DataQuery Query(ConnectionProvider provider)
			=> new DataQuery(new SqlDbAgent(provider));
	}
}
