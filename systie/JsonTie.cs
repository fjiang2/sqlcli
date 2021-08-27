using System;
using System.Collections.Generic;
using System.Text;
using Tie;

namespace Sys
{
	public static class JsonTie
	{
		public static T ReadObject<T>(this string json)
		{
			if (json == null)
				return default(T);

			var val = Script.Evaluate(json);
			return Valizer.Devalize<T>(val);
		}

		public static string WriteObject<T>(this T graph)
		{
			var val = Valizer.Valize(graph);
			return val.ToJson();
		}

		public static string ToSimpleString(this VAL val)
		{
			return val.ToSimpleString();
		}

	}
}
