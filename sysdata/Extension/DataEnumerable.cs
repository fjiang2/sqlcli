using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Sys.Data
{
    public static class DataEnumerable
    {
     

        public static DataTable ToTable<T>(this IEnumerable<T> records) where T : class, IDPObject, new()
        {
            DPList<T> list = new DPList<T>(records);
            return list.Table;
        }

        public static DPList<T> ToDPList<T>(this IEnumerable<T> collection) where T : class, IDPObject, new()
        {
            return new DPList<T>(collection);
        }
    }
}
