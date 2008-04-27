using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	internal class Pair<T1, T2> {
		private readonly T1 t1;
		private readonly T2 t2;

		public Pair(T1 t1, T2 t2) {
			this.t1 = t1;
			this.t2 = t2;
		}

		public override bool Equals(object obj) {
			if (obj == null) return false;
			if (!(obj is Pair<T1, T2>)) return false;

			Pair<T1, T2> p = (Pair<T1, T2>)obj;
			return t1.Equals(p.t1) &&
				t2.Equals(p.t2);
		}

		public override int GetHashCode() {
			return t1.GetHashCode() ^ t2.GetHashCode();
		}
	}

	public sealed class RandomComparer<T> : IComparer<T> where T : class {
		Dictionary<Pair<T, T>, int> cache = new Dictionary<Pair<T, T>, int>();

		public int Compare(T x, T y) {
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");

			Pair<T, T> p1 = new Pair<T, T>(x, y);
			if (cache.ContainsKey(p1)) return cache[p1];
			Pair<T, T> p2 = new Pair<T, T>(y, x);
			if (cache.ContainsKey(p2)) return -cache[p2];

			int res = 0;
			if (x != y) {
				double rnd = StaticRandom.NextDouble();
				res = rnd > 0.5 ? +1 : -1;
			}

			cache[p1] = res;
			return res;
		}
	}
}
