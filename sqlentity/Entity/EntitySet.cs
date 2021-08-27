using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys.Data.Entity
{
    public class EntitySet<TEntity> : List<TEntity>
        where TEntity : class
    {
        public EntitySet(IEnumerable<TEntity> entities)
            : base(entities)
        {

        }

    }
}
