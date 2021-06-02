using Sys.Data;

namespace sqlcli
{
    public interface ISide
    {
        DatabaseName DatabaseName { get; }
        ConnectionProvider Provider { get; }

        void UpdateDatabase(ConnectionProvider provider);
    }
}