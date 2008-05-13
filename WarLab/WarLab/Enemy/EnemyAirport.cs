using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	/// <summary>
	/// Вражеский аэродром, на котором находятся либо бомбардировщики, либо истребители
	/// </summary>
	public class EnemyAirport : Airport {
		protected override bool VerifyPlaneTypeCore(Plane plane) {
			return plane is EnemyPlane;
		}

		private double minHeight = 500;
		public double MinHeight {
			get { return minHeight; }
			set { minHeight = value; }
		}

		private double maxHeight = 2000;
		public double MaxHeight {
			get { return maxHeight; }
			set { maxHeight = value; }
		}

		protected override double PlaneHeight {
			get {
				double ratio = StaticRandom.NextDouble();
				return minHeight + ratio * (maxHeight - minHeight);
			}
		}
	}
}
