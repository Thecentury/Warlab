using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	public class EnemyBomber : EnemyPlane {
		public EnemyBomber() {
			PlaneImportance = Default.EnemyBomberImportance;
			Health = Default.EnemyBomberHealth;
		}
	}
}
