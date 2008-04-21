using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;

namespace EnemyPlanes {
	/// <summary>
	/// Вражеский истребитель
	/// </summary>
	public class EnemyFighter : EnemyPlane {
		#region vars
		private double maxSpeed;//максимальная скорость
		#endregion

		#region properies

		/// <summary>
		/// Максимальная скорость. На ней, например, самолет летит на базу
		/// </summary>
		public double MaxSpeed {
			get { return maxSpeed; }
			set { maxSpeed = value; }
		}

		#endregion

		/// <summary>
		/// Создать вражеский истребитель
		/// </summary>
		/// <param name="rockets">Количество ракет</param>
		/// <param name="fuel">Вместимость бака с топливом</param>
		/// <param name="speed">Максимальная скорость</param>
		public EnemyFighter(int rockets, double fuel, double speed)
			: base(rockets, fuel, speed) {
			this.MaxSpeed = speed;
		}

		#region Plane implementation

		protected override void UpdateCore(WarTime warTime) {

		}

		#endregion
	}
}
