using System.Collections.Generic;

namespace Ateo.Extensions
{
	public static class ListExtensions
	{
		/// <summary>
		/// Shuffles the element order of the specified list.
		/// </summary>
		public static void Shuffle<T>(this IList<T> ts)
		{
			int count = ts.Count;
			int last = count - 1;

			for (int i = 0; i < last; ++i)
			{
				int r = UnityEngine.Random.Range(i, count);
				(ts[i], ts[r]) = (ts[r], ts[i]);
			}
		}

		/// <summary> Get a random item from the target list </summary>
		/// <param name="target"> Target list </param>
		/// <typeparam name="T"> Item type </typeparam>
		/// <returns> Random item from list </returns>
		public static T GetRandomItem<T>(this List<T> target) =>
			target[UnityEngine.Random.Range(0, target.Count)];


		/// <summary> Remove null entries from the target list </summary>
		/// <param name="target"> Target list </param>
		/// <returns> The list without any null reference </returns>
		public static List<T> RemoveNulls<T>(this List<T> target)
		{
			for (int i = target.Count - 1; i >= 0; i--)
				if (target[i] == null)
					target.RemoveAt(i);

			return target;
		}
	}
}