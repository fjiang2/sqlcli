using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Sys.Data.Entity
{
	public static class EntityExtension
	{
		public static List<T> ToList<T>(this DataTable dt) where T : IEntityRow, new()
		{
			List<T> list = new List<T>();
			foreach (DataRow row in dt.Rows)
			{
				var obj = new T();
				obj.FillObject(row);
				list.Add(obj);
			}
			return list;
		}
	}
}
