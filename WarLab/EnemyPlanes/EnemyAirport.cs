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
	}
}
