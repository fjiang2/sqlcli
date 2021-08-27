using System;
using System.Data;
using System.Collections.Generic;

namespace Sys.Data.Entity
{
    class QueryResultReader : IQueryResultReader
    {
        private readonly DataContext db;
        private readonly Type[] types;
        private readonly DataSet ds;

        public QueryResultReader(DataContext db, Type[] types, DataSet ds)
        {
            this.db = db;
            this.types = types;
            this.ds = ds;
        }

        public IEnumerable<TEntity> Read<TEntity>() where TEntity : class
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (typeof(TEntity) == types[i])
                    return db.GetTable<TEntity>().ToList(ds.Tables[i]);
            }

            return null;
        }

        public override string ToString()
        {
            return ds.ToString();
        }
    }
}
