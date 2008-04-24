using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	/// <summary>
	/// Базовый класс для вражеских самолетов
	/// </summary>
	public abstract class EnemyPlane : Plane {
		public EnemyPlane(int weapons) : base(weapons) { }
		public EnemyPlane() { }
	}
}
