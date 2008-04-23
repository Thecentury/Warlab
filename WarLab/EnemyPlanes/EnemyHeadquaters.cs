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

		#region vars
		//private Dictionary<EnemyBomber,EnemyBomberAI> bombers;
		//private Dictionary<EnemyFighter,EnemyFighterAI> fighters;
		/// <summary>
		/// Аэропорт бомбардировщиков
		/// </summary>
		private EnemyAirport bombersAirport;
		/// <summary>
		/// Аэропорт истребителей
		/// </summary>
		private EnemyAirport fightersAirport;
		#endregion

		/// <summary>
		/// Создать командный пункт для вражеских самолетов
		/// </summary>
		/// <param name="bombersAirport">Аэродром для бомбардировщиков</param>
		/// <param name="fightersAirport">Аэродром для истребителей</param>
		public EnemyHeadquaters(EnemyAirport bombersAirport, EnemyAirport fightersAirport) {
			if (bombersAirport == null)
				throw new ArgumentNullException("bombersAirport");
			if (fightersAirport == null)
				throw new ArgumentNullException("fightersAirport");

			//bombers = new Dictionary<EnemyBomber,EnemyBomberAI>();
			//fighters = new Dictionary<EnemyFighter,EnemyFighterAI>();
			this.bombersAirport = bombersAirport;
			this.fightersAirport = fightersAirport;
		}

		#region properties

		/// <summary>
		/// Получить аэродром вражеских бомбардировщиков
		/// </summary>
		public EnemyAirport BombersAirport {
			get { return bombersAirport; }
		}

		/// <summary>
		/// Получить аэродром вражеских истребителей
		/// </summary>
		public EnemyAirport FightersAirport {
			get { return fightersAirport; }
		}

		#endregion

		private void AnalyzeTargets() {
			var ourTargets = World.SelectAll<IHasImportance>();
			double sum = ourTargets.Sum(t => t.Importance);
		}
	
		/// <summary>
		/// Навести бомбадировщик на цель.
		/// </summary>
		/// <param name="plane">Бомбардировщик</param>
		/// <param name="target">Цель</param>
		public void Navigate(EnemyBomber plane, StaticTarget target) {
			EnemyBomberAI ai = (EnemyBomberAI)plane.AI;
			ai.AttackTarget(target);
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
				foreach (EnemyFighter fighter in fightersAirport.PlanesOfType<EnemyFighter>()) {
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
	}
}

