using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	/// <summary>
	/// Вражеский истребитель
	/// </summary>
	public class EnemyFighter : EnemyPlane {
		/// <summary>
		/// максимальная скорость
		/// </summary>
		private readonly double maxSpeed;

		public EnemyFighter(int weapons, double fuel, double maxspeed)
			: base(weapons) {
			this.maxSpeed = maxspeed;
			this.FuelLeft = fuel;
		}

		/// <summary>
		/// Максимальная скорость. На ней, например, самолет летит на базу.
		/// </summary>
		public double MaxSpeed {
			get { return maxSpeed; }
			//internal set
			//{
			//    Verify.IsPositive(value);

			//    maxSpeed = value;
			//}
		}
	}
}
