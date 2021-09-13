namespace Sys.Data
{
	public class SqlDbAgent : IDbAgent
	{
		public ConnectionProvider provider { get; }

		public SqlDbAgent(ConnectionProvider provider)
		{
			this.provider = provider;
		}

		public IDbCmd Proxy(SqlUnit unit)
			=> new SqlCmd(provider, unit);

		public DbAgentOption Option => new DbAgentOption { Style = provider.AgentStyle() };
	}
}
