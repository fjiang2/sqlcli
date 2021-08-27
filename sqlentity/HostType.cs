using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Sys.Data
{
	class HostType
	{
		public static Type GetType(Type type, string name)
		{
			return Assembly.GetAssembly(type).GetType(name);
		}
	}
}
