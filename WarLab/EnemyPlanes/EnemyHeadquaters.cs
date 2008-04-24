using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	/// <summary>
	/// Командный пункт вражеских самолетов
	/// </summary>
	public class EnemyHeadquaters : StaticObject {

		private void AnalyzeTargets() {
			foreach (var airport in World.SelectAll<EnemyAirport>()) {
				var mostImportantTarget = GetMostImportantTarget();

				if (mostImportantTarget != null && airport.CanLaunch<EnemyBomber>()) {
					EnemyBomber bomber = airport.LaunchPlane<EnemyBomber>();

					var bomberAI = (EnemyBomberAI)bomber.AI;
					bomberAI.TargetDestroyed += EnemyHeadquaters_TargetReached;

					Attack(bomberAI, mostImportantTarget);
				}
			}
		}

		private OurStaticObject GetMostImportantTarget() {
			var ourTargets = World.SelectAll<OurStaticObject>().Where(t => t.Health > 0).ToList();
			if (ourTargets.Count == 0) return null;

			double sum = ourTargets.Sum(t => t.Importance);

			double rnd = StaticRandom.NextDouble();
			double s = 0;
			for (int i = 0; i < ourTargets.Count; i++) {
				if (s + ourTargets[i].Importance / sum > rnd) {
					return ourTargets[i];
				}
				else {
					s += ourTargets[i].Importance / sum;
				}
			}

			return ourTargets[0];
		}

		private readonly Dictionary<OurStaticObject, List<EnemyBomberAI>> assignedTargets = new Dictionary<OurStaticObject, List<EnemyBomberAI>>();

		private void EnemyHeadquaters_TargetReached(object sender, TargetDestroyedEventArgs args) {
			EnemyBomberAI bomberAI = sender as EnemyBomberAI;
			RetargetBomber(bomberAI);
		}

		private void RetargetBomber(EnemyBomberAI bomberAI) {
			var mostImportantTarget = GetMostImportantTarget();
			if (mostImportantTarget != null) {
				Attack(bomberAI, mostImportantTarget);
			}
			else {
				bomberAI.Mode = BomberFlightMode.ReturnToBase;
			}
		}

		protected override void UpdateImpl(WarTime time) {
			AnalyzeTargets();
		}

		/// <summary>
		/// Навести бомбадировщик на цель.
		/// </summary>
		/// <param name="plane">Бомбардировщик</param>
		/// <param name="target">Цель</param>
		public void Attack(EnemyBomberAI planeAI, OurStaticObject target) {
			if (!assignedTargets.ContainsKey(target)) {
				target.Destroyed += target_Destroyed;

				List<EnemyBomberAI> assignedAIs = new List<EnemyBomberAI>(1);
				assignedAIs.Add(planeAI);

				assignedTargets[target] = assignedAIs;
			}
			else {
				var assignedAIs = assignedTargets[target];
				assignedAIs.Add(planeAI);
			}
			planeAI.AttackTarget(target);
		}

		private void target_Destroyed(object sender, EventArgs e) {
			OurStaticObject destroyedTarget = (OurStaticObject)sender;
			destroyedTarget.Destroyed -= target_Destroyed;

			var assignedAIs = assignedTargets[destroyedTarget];

			assignedTargets.Remove(destroyedTarget);

			foreach (var ai in assignedAIs) {
				RetargetBomber(ai);
			}
		}

		/// <summary>
		/// Навести истребитель
		/// </summary>
		/// <param name="plane">Истребитель</param>
		/// <param name="target">Бомбардировщик или истребитель обороняющейся стороны</param>
		public void Navigate(EnemyFighter plane, Plane target) {
			//EnemyFighterAI ai = fighters[plane];
			if (target is EnemyBomber) {
				List<EnemyFighter> followingFighters = new List<EnemyFighter>();// истребители, уже сопровождающие этот бомбер
				EnemyBomber bomber = (EnemyBomber)target;
				EnemyBomberAI bomberAI = (EnemyBomberAI)bomber.AI;
				double radius = bomberAI.FightersRadius;

				//находим все самолеты, уже сопровождающие этот бомбер
				foreach (EnemyFighter fighter in World.SelectAll<EnemyFighter>()) {
					if (((EnemyFighterAI)fighter.AI).Target == target)
						followingFighters.Add(fighter);
				}

				// добавим в число этих истребителей наводимый в этой функции истребитель
				followingFighters.Add(plane);
				double delta = 360.0 / (double)followingFighters.Count; //угол между истребителями
				for (int i = 0; i < followingFighters.Count; i++) {
					// задаем смещение истребителю и наводим его
					double angle = i * delta;
					((EnemyFighterAI)followingFighters[i].AI).FollowBomber(bomber, angle);
				}
			}
			else
				((EnemyFighterAI)plane.AI).AttackFighter(target);
		}

		public void Navigate(List<EnemyFighter> planes, Plane target) {
			//EnemyFighterAI ai = fighters[plane];
			if (target is EnemyBomber) {
				EnemyBomber bomber = (EnemyBomber)target;
				EnemyBomberAI bomberAI = (EnemyBomberAI)bomber.AI;
				double radius = bomberAI.FightersRadius;

				double delta = 360.0 / planes.Count; //угол между истребителями
				for (int i = 0; i < planes.Count; i++) {
					// задаем смещение истребителю и наводим его
					double angle = i * delta;
					((EnemyFighterAI)planes[i].AI).FollowBomber(bomber, angle);
				}
			}
			else {
				// todo доделать
				//((EnemyFighterAI)planes.AI).AttackFighter(target);
			}
		}


	}
}

