using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class Default {
		public static readonly double EnemyBomberHealth = 10;
		public static readonly double EnemyFighterHealth = 10;

		public static readonly double EnemyBomberImportance = 3;

		public static readonly int EnemyFighterWeapons = 10;

		public static readonly int OurFighterWeapons = 30;

		public static readonly double FighterSpeed = Speed.FromKilometresPerHour(40);
	}
}
