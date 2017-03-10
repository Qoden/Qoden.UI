using System.Collections.Generic;
#pragma warning disable CS1701 // Assuming assembly reference matches identity
namespace Qoden.UI
{
	public class Pool<T>
	{
		readonly Dictionary<string, List<T>> pool = new Dictionary<string, List<T>> ();

		public Pool()
		{
		}

		public void Enqueue (T element, string reuseId)
		{
			if (reuseId != null) {
				List<T> list;
				if (!pool.ContainsKey (reuseId)) {
					pool [reuseId] = list = new List<T> ();
				} else {
					list = pool [reuseId];
				}
				if (!list.Contains (element)) {
					list.Add (element);
				}
			}
		}

		public T Deque (string reuseId)
		{
			if (pool.ContainsKey (reuseId)) {
				var list = pool [reuseId];
				if (list.Count > 0) {
					var element = list [list.Count - 1];
					list.RemoveAt (list.Count - 1);
					return element;
				} 
			}
			return default(T);
		}

		public void Clear ()
		{
			pool.Clear ();
		}
	}
}

