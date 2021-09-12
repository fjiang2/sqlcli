namespace Sys.Data
{
	public class SqlDbAgent : IDbAgent
	{
		public ConnectionProvider provider { get; }

		public SqlDbAgent(ConnectionProvider provider)
		{
			this.provider = provider;
		}

		public IDbCmd Command(string sql, object args)
			=> new SqlCmd(provider, sql, args);

		public DbAgentOption Option => new DbAgentOption { Style = provider.AgentStyle() };
		public DbCmdFunction Function => Command;
	}
}
