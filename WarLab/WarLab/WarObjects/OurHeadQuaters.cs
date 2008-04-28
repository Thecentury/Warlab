using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace WarLab.WarObjects {
	public sealed class OurHeadquaters : WarObject {

		protected override void UpdateImpl(WarTime time) {
			RetargetIfTargetLanded();

			// поиск новых целей
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();
			var ourAirports = World.SelectAll<OurAirport>().ToList();

			// перемешиваем врагов случайным образом
			// enemyPlanes.Sort(new RandomComparer<EnemyPlane>());

			enemyPlanes.Sort((p1, p2) => -p1.PlaneImportance.CompareTo(p2.PlaneImportance));

			// поднимаем самолеты в воздух
			if (ourAirports.Count != 0) {
				foreach (var plane in enemyPlanes) {
					Vector3D planePosition = plane.Position;

					// ищем ближайший к вражескому самолету аэродром.
					ourAirports.Sort((a1, a2) => a1.Position.Distance2D(planePosition).CompareTo(a2.Position.Distance2D(planePosition)));
					foreach (var airport in ourAirports) {
						if (!IsAssignedTooMuchPlanes(plane) && airport.CanLaunch<OurFighter>()) {

							var ourFighter = airport.LaunchPlane<OurFighter>();
							ourFighter.Destroyed += OnOurFighterDestroyed;
							OurFighterAI ai = (OurFighterAI)ourFighter.AI;

							AssignPlane(plane, ai);
							Verify.IsTrue(ai.TargetPlane != null);
						}
					}

					// ищем незанятые самолеты в воздухе и перенацеливаем их.
					var freePlanes = ourAirports.SelectMany(airport => airport.Planes).
						Where(pi => pi.State == AirportPlaneState.InAir && ((OurFighterAI)pi.Plane.AI).CanRetarget).
						Select(pi => pi.Plane).ToList();

					if (freePlanes.Count > 0) {
						freePlanes.Sort((p1, p2) => p1.Position.Distance2D(planePosition).CompareTo(p2.Position.Distance2D(planePosition)));
						foreach (var freePlane in freePlanes) {
							AssignPlane(plane, (OurFighterAI)freePlane.AI);
						}
					}
				}
			}
		}

		private void RetargetIfTargetLanded() {
			List<EnemyPlane> landedPlanes = assignedPlanes.Keys.Where(p => p.IsLanded).ToList();

			foreach (var enemyPlane in landedPlanes) {
				RetargetAssignedTo(enemyPlane);
			}
		}

		private void OnOurFighterDestroyed(object sender, EventArgs e) {
			OurFighter fighter = (OurFighter)sender;
			fighter.Destroyed -= OnOurFighterDestroyed;
			var fighterAI = (OurFighterAI)fighter.AI;

			assignedPlanes.Values.Where(planes => planes.Contains(fighterAI)).
				ToList().ForEach(planes => planes.Remove(fighterAI));
		}

		private void OnEnemyPlaneDestroyed(object sender, EventArgs e) {
			EnemyPlane plane = (EnemyPlane)sender;

			RetargetAssignedTo(plane);
		}

		private void RetargetAssignedTo(EnemyPlane plane) {
			plane.Destroyed -= OnEnemyPlaneDestroyed;

			List<OurFighterAI> planes = assignedPlanes[plane];
			assignedPlanes.Remove(plane);
			foreach (var fighter in planes) {
				RetargetFighter(fighter);
			}
		}

		private void RetargetFighter(OurFighterAI fighterAI) {
			var closestEnemies = GetClosestEnemyPlanes(fighterAI.ControlledPlane);

			bool assigned = false;
			foreach (var enemyPlane in closestEnemies) {
				if (AssignPlane(enemyPlane, fighterAI)) {
					assigned = true;
					break;
				}
			}
			if (!assigned) {
				fighterAI.ReturnToBase();
			}
		}

		private IEnumerable<EnemyPlane> GetVisibleEnemyPlanes() {
			var rls = World.SelectSingle<RLS>();
			if (rls == null) return new List<EnemyPlane>();

			return rls.PlanesInCoverage;
		}

		private List<EnemyPlane> GetClosestEnemyPlanes(Plane plane) {
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();

			//if (enemyPlanes.Count == 0) return null;
			Vector3D planePos = plane.Position;
			enemyPlanes.Sort((p1, p2) => p1.Position.Distance2D(planePos).CompareTo(p2.Position.Distance2D(planePos)));

			//return enemyPlanes[0];
			return enemyPlanes;
		}

		Dictionary<EnemyPlane, List<OurFighterAI>> assignedPlanes = new Dictionary<EnemyPlane, List<OurFighterAI>>();

		private readonly int maxPlanesPerTarget = 3;

		private bool AssignPlane(EnemyPlane target, OurFighterAI plane) {
			if (!assignedPlanes.ContainsKey(target)) {
				assignedPlanes[target] = new List<OurFighterAI>(1);
				target.Destroyed -= OnEnemyPlaneDestroyed;
				target.Destroyed += OnEnemyPlaneDestroyed;
			}

			List<OurFighterAI> planesForTarget = assignedPlanes[target];

			// на данную цель уже нацелено слишком много самолетов
			// if (planesForTarget.Count > target.PlaneImportance * maxPlanesPerTarget) return false;
			if (planesForTarget.Contains(plane)) return true;

			if (plane.AttackTarget(target)) {
				planesForTarget.Add(plane);
				return true;
			}
			return false;
		}

		private bool IsAssignedTooMuchPlanes(EnemyPlane target) {
			return assignedPlanes.ContainsKey(target) &&
				assignedPlanes[target].Count >= target.PlaneImportance * maxPlanesPerTarget;
		}
	}
}
