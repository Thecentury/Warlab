using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	/// <summary>
	/// Вражеский аэродром, на котором находятся либо бомбардировщики, либо истребители
	/// </summary>
	public class EnemyAirport : Airport {
		protected override bool VerifyPlaneTypeCore(Plane plane) {
			return plane is EnemyPlane;
		}

		protected override void LaunchPlaneCore(Plane plane) {
			EnemyBomber bomber = plane as EnemyBomber;
			List<EnemyFighter> fighters = new List<EnemyFighter>();
			for (int i = 0; i < 3; i++) {
				EnemyFighter fighter = new EnemyFighter(10, bomber.FuelLeft, Speed.FromKilometresPerHour(500));
				World.AddWarObject(fighter, bomber.Position);

				fighters.Add(fighter);
			}

			EnemyHeadquaters hq = World.SelectSingle<EnemyHeadquaters>();
			if (hq != null) {
				hq.Navigate(fighters, plane);
			}
		}
	}
}
