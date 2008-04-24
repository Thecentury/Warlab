using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	public class EnemyBomber : EnemyPlane {
		public EnemyBomber(int weapons, double fuel, double speed)
			: base(weapons) {
			this.FuelLeft = fuel;
			this.Speed = speed;
		}
	}
}
