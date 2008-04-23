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
		private double maxSpeed;

		/// <summary>
		/// Максимальная скорость. На ней, например, самолет летит на базу.
		/// </summary>
		public double MaxSpeed {
			get { return maxSpeed; }
			set {
				Verify.IsPositive(value);

				maxSpeed = value;
			}
		}
	}
}
