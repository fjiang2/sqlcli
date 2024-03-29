﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sys
{
	public static class Iteratable
	{
		public static IEnumerable<T> Enumerable<T>(params T[] items) => items;

		public static void ForEach<TSource>(this IEnumerable<TSource> items, Action<TSource> action, Action<TSource> delimiter)
		{
			bool first = true;

			foreach (var item in items)
			{
				if (!first)
					delimiter(item);

				first = false;
				action(item);
			}
		}

		public static void ForEach<TSource>(this IEnumerable<TSource> items, Action<TSource> action)
		{
			foreach (var item in items)
			{
				action(item);
			}
		}

		public static string Join<TSource>(this IEnumerable<TSource> items, Func<TSource, string> selector, string delimiter)
		{
			StringBuilder builder = new StringBuilder();
			items.ForEach(
				item => builder.Append(selector(item)),
				_ => builder.Append(delimiter)
			 );

			return builder.ToString();
		}


		/// <summary>
		/// </summary>
		/// <typeparam name="TResult"></typeparam>
		/// <param name="text"></param>
		/// <param name="convert">convert substring to typeof(T)</param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static IEnumerable<TResult> Split<TResult>(this string text, Func<string, TResult> convert, string separator)
		{
			string[] items = text.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

			List<TResult> list = new List<TResult>();

			foreach (var item in items)
			{
				list.Add(convert(item));
			}

			return list;
		}

		public static IEnumerable<TResult> Split<TResult>(this string text, Func<string, TResult> convert)
		{
			return Split(text, convert, ",");
		}

	}
}
