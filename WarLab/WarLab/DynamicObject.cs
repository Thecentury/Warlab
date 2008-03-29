using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого подвижного объекта мира.
	/// </summary>
	public abstract class DynamicObject : WarObject, ISpriteSource{
		private double speed;
		public double Speed {
			get { return speed; }
			internal set { speed = value; }
		}

		private Vector2D orientation;
		public Vector2D Orientation {
			get { return orientation; }
			internal set { orientation = value; }
		}

	}
}
