using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EnemyPlanes;
using WarLab.WarObjects;

namespace WarLab.SampleUI.Charts {
	public class Finish : WarObject {
		protected override void UpdateImpl(WarTime time) {
			var hasPorts = World.SelectAll<StaticTarget>().Any();
			if (!hasPorts) {
				StopWar("Порты уничтожены");
				return;
			}

			bool hasEnemies = World.SelectAll<EnemyAirport>().SelectMany(a => a.Planes).
				Where(ai => ai.Plane is EnemyBomber && ai.State != AirportPlaneState.Dead).
				Any();
			if (!hasEnemies) {
				StopWar("Враги уничтожены");
				return;
			}

			bool hasRlses = World.SelectAll<RLS>().Any();
			if (!hasRlses) {
				StopWar("РЛС уничтожены");
				return;
			}

			bool hasZrks = World.SelectAll<ZRK>().Any();
			bool hasOurPlanes = World.SelectAll<OurAirport>().SelectMany(a => a.Planes).
				Where(ai => ai.State != AirportPlaneState.Dead).Any();
			if (!(hasZrks || hasOurPlanes)) {
				StopWar("Защита уничтожена");
				return;
			}
		}

		private void StopWar(string p) {
			World.GetTimeControl().Stop();
			Debug.WriteLine(p);
		}
	}
}
