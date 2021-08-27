using System.Collections.Generic;

namespace Sys.Data.Entity
{
    public interface IQueryResultReader
    {
        IEnumerable<TEntity> Read<TEntity>() where TEntity : class;
    }
}