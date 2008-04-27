using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace WarLab.WarObjects {
	public sealed class OurHeadquaters : WarObject {

		protected override void UpdateImpl(WarTime time) {
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();
			var ourAirports = World.SelectAll<OurAirport>().ToList();

			// перемешиваем врагов случайным образом
			enemyPlanes.Sort(new RandomComparer<EnemyPlane>());

			// поднимаем самолеты в воздух
			if (ourAirports.Count != 0) {
				foreach (var plane in enemyPlanes) {
					Vector3D planePosition = plane.Position;

					// ищем ближайший к вражескому самолету аэродром.
					ourAirports.Sort((a1, a2) => a1.Position.Distance2D(planePosition).CompareTo(a2.Position.Distance2D(planePosition)));
					foreach (var airport in ourAirports) {
						if (airport.CanLaunch<OurFighter>()) {

							var ourFighter = airport.LaunchPlane<OurFighter>();
							OurFighterAI ai = (OurFighterAI)ourFighter.AI;

							AssignPlane(plane, ai);
						}
					}

					// ищем незанятые самолеты в воздухе и перенацеливаем их.
					var freePlanes = ourAirports.SelectMany(airport => airport.Planes).
						Where(pi => pi.State == AirportPlaneState.InAir && ((OurFighterAI)pi.Plane.AI).CanRetarget).
						Select(pi => pi.Plane).ToList();

					if (freePlanes.Count > 0) {
						freePlanes.Sort((p1, p2) => p1.Position.Distance2D(planePosition).CompareTo(p2.Position.Distance2D(planePosition)));
						var freePlane = freePlanes[0];

						AssignPlane(plane, (OurFighterAI)freePlane.AI);
					}
				}
			}
		}

		private void OnEnemyPlaneDestroyed(object sender, EventArgs e) {
			EnemyPlane plane = (EnemyPlane)sender;
			plane.Destroyed -= OnEnemyPlaneDestroyed;

			List<OurFighterAI> planes = assignedPlanes[plane];
			assignedPlanes.Remove(plane);
			foreach (var fighter in planes) {
				RetargetFighter(fighter);
			}
		}

		private void RetargetFighter(OurFighterAI fighterAI) {
			var closestEnemy = GetClosestEnemyPlane(fighterAI.ControlledPlane);

			if (closestEnemy != null) {
				AssignPlane(closestEnemy, fighterAI);
			}
			else {
				fighterAI.ReturnToBase();
			}
		}

		private IEnumerable<EnemyPlane> GetVisibleEnemyPlanes() {
			var rls = World.SelectSingle<RLS>();
			if (rls == null) return new List<EnemyPlane>();

			return rls.GetPlanesInCoverage();
		}

		private EnemyPlane GetClosestEnemyPlane(Plane plane) {
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();

			if (enemyPlanes.Count == 0) return null;
			Vector3D planePos = plane.Position;
			enemyPlanes.Sort((p1, p2) => p1.Position.Distance2D(planePos).CompareTo(p2.Position.Distance2D(planePos)));

			return enemyPlanes[0];
		}

		Dictionary<EnemyPlane, List<OurFighterAI>> assignedPlanes = new Dictionary<EnemyPlane, List<OurFighterAI>>();

		private readonly int maxPlanesPerTarget = 3;

		private void AssignPlane(EnemyPlane target, OurFighterAI plane) {
			if (!assignedPlanes.ContainsKey(target)) {
				assignedPlanes[target] = new List<OurFighterAI>(1);
				target.Destroyed -= OnEnemyPlaneDestroyed;
				target.Destroyed += OnEnemyPlaneDestroyed;
			}

			List<OurFighterAI> planesForTarget = assignedPlanes[target];

			// на данную цель уже нацелено слишком много самолетов
			if (planesForTarget.Count >= target.PlaneImportance * maxPlanesPerTarget) return;

			if (plane.AttackTarget(target)) {
				planesForTarget.Add(plane);
			}
		}
	}
}
