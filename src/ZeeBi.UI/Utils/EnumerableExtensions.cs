using System;
using System.Collections.Generic;
using System.Linq;

namespace ZeeBi.UI.Utils
{
	public static class EnumerableExtensions
	{
		/// <summary>
		/// Projects the sequence as a list of <see cref="KeyValuePair{TKey,TValue}"/> objects.
		/// </summary>
		/// <param name="target">The sequence to project.</param>
		/// <param name="keySelector">A function to select a key from an element in the sequence.</param>
		/// <param name="valueSelector">A function to select a value from an element in the sequence.</param>
		/// <returns>A sequence of <see cref="KeyValuePair{TKey,TValue}"/> objects 
		/// created from the elements in the target sequence by applying the key and value selector functions.</returns>
		public static IEnumerable<KeyValuePair<TKey, TValue>> ToKeyValuePairs<T, TKey, TValue>(
			this IEnumerable<T> target,
			Func<T, TKey> keySelector,
			Func<T, TValue> valueSelector)
		{
			return target.Select(x => new KeyValuePair<TKey, TValue>(keySelector(x), valueSelector(x)));
		}
	}
}