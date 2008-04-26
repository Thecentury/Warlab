using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace WarLab.WarObjects {
	public sealed class OurHeadQuaters : OurStaticObject {
		protected override void UpdateImpl(WarTime time) {
			var enemyPlanes = World.SelectAll<EnemyPlane>();
			var ourAirports = World.SelectAll<OurAirport>().ToList();

			if (ourAirports.Count == 0) return;

			foreach (var plane in enemyPlanes) {
				Vector3D planePosition = plane.Position;
				ourAirports.Sort((a1, a2) => a1.Position.Distance2D(planePosition).CompareTo(a2.Position.Distance2D(planePosition)));

				var closestAirport = ourAirports[0];
				if (closestAirport.CanLaunch<OurFighter>()) {
					var ourFighter = closestAirport.LaunchPlane<OurFighter>();
					OurFighterAI ai = (OurFighterAI)ourFighter.AI;
					ai.AttackFighter(plane);
				}
			}
		}
	}
}
