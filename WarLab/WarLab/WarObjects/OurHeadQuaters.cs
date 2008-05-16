using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using EnemyPlanes;

namespace WarLab.WarObjects {
	public sealed class OurHeadquaters : WarObject {

		protected override string NameCore {
			get {
				return "КП обороны";
			}
		}

		protected override void UpdateImpl(WarTime time) {
			RetargetIfTargetLanded();

			// поиск новых целей
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();
			var ourAirports = World.SelectAll<OurAirport>().ToList();

			// перемешиваем врагов случайным образом
			// enemyPlanes.Sort(new RandomComparer<EnemyPlane>());

			enemyPlanes.Sort((p1, p2) => -p1.PlaneImportance.CompareTo(p2.PlaneImportance));

			// поднимаем самолеты в воздух
			if (ourAirports.Count == 0) return;

			foreach (var plane in enemyPlanes) {
				Vector3D planePosition = plane.Position;

				// ищем ближайший к вражескому самолету аэродром.
				ourAirports.Sort((a1, a2) => a1.Position.Distance2D(planePosition).CompareTo(a2.Position.Distance2D(planePosition)));
				foreach (var airport in ourAirports) {
					if (!IsAssignedTooMuchPlanes(plane) &&
						airport.CanLaunch<OurFighter>() &&
						!CanBeAssignedToZRK(plane)
						) {

						var ourFighter = airport.LaunchPlane<OurFighter>();
						ourFighter.Destroyed += OnOurFighterDestroyed;
						OurFighterAI ai = (OurFighterAI)ourFighter.AI;

						if (AssignPlane(plane, ai)) {
							Verify.IsTrue(ai.TargetPlane != null);
						}
						else {
							airport.LandPlane(ourFighter);
						}
					}
				}

				// ищем незанятые самолеты в воздухе и перенацеливаем их.
				var freePlanes = ourAirports.SelectMany(airport => airport.Planes).
					Where(pi => pi.State == AirportPlaneState.InAir && ((OurFighterAI)pi.Plane.AI).CanRetarget(plane)).
					Select(pi => pi.Plane).ToList();

				if (freePlanes.Count > 0) {
					freePlanes.Sort((p1, p2) => p1.Position.Distance2D(planePosition).CompareTo(p2.Position.Distance2D(planePosition)));
					foreach (var freePlane in freePlanes) {
						AssignPlane(plane, (OurFighterAI)freePlane.AI);
					}
				}
			}
		}

		private void RetargetIfTargetLanded() {
			List<EnemyPlane> landedPlanes = assignedPlanes.Keys.Where(p => p.IsLanded).ToList();

			foreach (var enemyPlane in landedPlanes) {
				RetargetOurAssignedToEnemy(enemyPlane);
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

			RetargetOurAssignedToEnemy(plane);
		}

		private void RetargetOurAssignedToEnemy(EnemyPlane plane) {
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
			return World.SelectAll<RLS>().SelectMany(rls => rls.PlanesInCoverage);
		}

		private List<EnemyPlane> GetClosestEnemyPlanes(Plane plane) {
			var enemyPlanes = GetVisibleEnemyPlanes().ToList();

			Vector3D planePos = plane.Position;
			enemyPlanes.Sort((p1, p2) => p1.Position.Distance2D(planePos).CompareTo(p2.Position.Distance2D(planePos)));

			return enemyPlanes;
		}

		private bool CanBeAssignedToZRK(EnemyBomber enemy) {
			EnemyBomberAI ai = (EnemyBomberAI)enemy.AI;
			if (ai.Mode == BomberFlightMode.ReturnToBase) return false;

			Vector3D targetPos = ai.Target.Position;

			Ray ray = new Ray(enemy.Position, targetPos - enemy.Position);

			// есть ли ЗРК, которые могут атаковать эту цель.
			bool res = World.Instance.SelectAll<ZRKBase>().
				Where(zrk => AllOKWithZRK(ray, zrk)).Any();
			if (!res) { }
			return res;
		}

		private bool AllOKWithZRK(Ray ray, ZRKBase zrk) {
			Vector3D zrkPos = zrk.Position;
			bool res = RayCoeff(zrkPos, ray) > 0 && Distance2DFromPointToRay(zrkPos, ray) <= zrk.CoverageRadius && zrk.NumOfEquipment > 0;
			if (!res) { }
			return res;
		}

		private bool CanBeAssignedToZRK(EnemyFighter enemy) {
			EnemyBomber bomber = ((EnemyFighterAI)enemy.AI).Bomber;
			return CanBeAssignedToZRK(bomber);
		}

		private bool CanBeAssignedToZRK(EnemyPlane enemy) {
			bool res;
			if (enemy is EnemyBomber) {
				res = CanBeAssignedToZRK((EnemyBomber)enemy);
			}
			else {
				res = CanBeAssignedToZRK((EnemyFighter)enemy);
			}

			bool isInZRKCoverage = World.SelectAll<ZRKBase>().Any(zrk => zrk.IsInCoverage(enemy) && zrk.NumOfEquipment > 0);
			return res || isInZRKCoverage;
		}

		class Ray {
			public Vector3D Origin;
			public Vector3D Dir;
			public Ray() { }
			public Ray(Vector3D origin, Vector3D direction) {
				this.Origin = origin;
				this.Dir = direction.Normalize();
			}
		}

		private static double RayCoeff(Vector3D point, Ray ray) {
			Vector3D v = ray.Origin.Projection2D - point;
			Vector3D dir = ray.Dir.Projection2D;
			double t = -(v & dir) / (dir & dir);
			return t;
		}

		private static Vector3D ClosestPointOnRay(Vector3D point, Ray ray) {
			double t = RayCoeff(point, ray);

			Vector3D a = ray.Origin + t * ray.Dir;
			return a;
		}

		private static double Distance2DFromPointToRay(Vector3D point, Ray ray) {
			Vector3D a = ClosestPointOnRay(point, ray);
			Vector3D toRay = a - point;
			return toRay.Projection2D.Length;
		}

		Dictionary<EnemyPlane, List<OurFighterAI>> assignedPlanes = new Dictionary<EnemyPlane, List<OurFighterAI>>();

		private readonly int maxPlanesPerTarget = 2;

		private bool AssignPlane(EnemyPlane target, OurFighterAI plane) {
			if (CanBeAssignedToZRK(target)) return false;
			EnemyBomber bomber = target as EnemyBomber;
			if (bomber != null) {
				EnemyBomberAI ai = (EnemyBomberAI)bomber.AI;
				if (ai.Mode == BomberFlightMode.ReturnToBase) return false;
			}

			if (!assignedPlanes.ContainsKey(target)) {
				assignedPlanes[target] = new List<OurFighterAI>(1);
				target.Destroyed -= OnEnemyPlaneDestroyed;
				target.Destroyed += OnEnemyPlaneDestroyed;
			}

			List<OurFighterAI> planesForTarget = assignedPlanes[target];

			// на данную цель уже нацелено слишком много самолетов
			// if (planesForTarget.Count > target.PlaneImportance * maxPlanesPerTarget) return false;
			if (IsAssignedTooMuchPlanes(target)) return false;
			if (planesForTarget.Contains(plane)) return plane.AttackTarget(target);

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
