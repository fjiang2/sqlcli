using System.Collections.Generic;

namespace Sys.Data.Linq
{
    public interface IQueryResultReader
    {
        IEnumerable<TEntity> Read<TEntity>() where TEntity : class;
    }
}