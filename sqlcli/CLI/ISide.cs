﻿using Sys.Data;

namespace Sys.Cli
{
    public interface ISide
    {
        DatabaseName DatabaseName { get; }
        ConnectionProvider Provider { get; }

        void UpdateDatabase(ConnectionProvider provider);
    }
}