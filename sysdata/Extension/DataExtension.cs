using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Sys.Data.Text;

namespace Sys.Data
{
	public static class DataExtension
	{
		/// <summary>
		/// Adjuested Length
		/// </summary>
		public static int AdjuestedLength(this IColumn column)
		{
			if (column.Length == -1)
				return -1;

			switch (column.CType)
			{
				case CType.NChar:
				case CType.NVarChar:
					return column.Length / 2;
			}

			return column.Length;
		}




		public static TableName TableName(this Type dpoType)
		{
			TableAttribute[] A = dpoType.GetAttributes<TableAttribute>();
			if (A.Length > 0)
				return A[0].TableName;
			else
				return null;
		}

		public static SqlCmd SqlCmd(this SqlBuilder sql, ConnectionProvider provider) => new SqlCmd(provider, sql.ToScript(DbAgentStyle.SqlServer));


		public static bool Invalid(this SqlBuilder sql, TableName tname)
		{
			bool result = false;

			sql.SqlCmd(tname.Provider).Error += (sender, e) =>
			{
				result = true;
			};

			try
			{
				sql.SqlCmd(tname.Provider).ExecuteScalar();

				return result;
			}
			catch (Exception)
			{
				return true;
			}
		}

	}
}
