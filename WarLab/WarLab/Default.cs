using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class Default {
		public static double EnemyBomberHealth = 2; // 3
		public static double EnemyFighterHealth = 1; // 2

		public static double OurFighterHealth = 1; // 2

		public static double EnemyBomberImportance = 3;

		public static int EnemyFighterWeapons = 10;

		public static int OurFighterWeapons = 30;

		public static double FighterSpeed = Speed.FromKilometresPerHour(40);

		public static double ZRKImportance = 10;

		public static double FighterRocketDamage = 1;
		public static double FighterRocketDamageRange = Distance.FromMetres(30);

		public static int OurAirportNumOfChannels = 10;
		public static TimeSpan EnemyBomberManeuverDuration = TimeSpan.FromSeconds(30);
	}
}
