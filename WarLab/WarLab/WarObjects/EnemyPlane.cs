using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	/// <summary>
	/// Базовый класс для вражеских самолетов
	/// </summary>
	public abstract class EnemyPlane : Plane {
		private double planeImportance = 1;
		public double PlaneImportance {
			get { return planeImportance; }
			protected set { planeImportance = value; }
		}
	}
}
