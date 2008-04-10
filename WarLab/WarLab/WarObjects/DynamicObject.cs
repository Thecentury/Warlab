﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого подвижного объекта мира.
	/// </summary>
	public abstract class DynamicObject : WarObject, ISpriteSource {
		private double speed = 10;
		public double Speed {
			get { return speed; }
			set { speed = value; }
		}

		private Vector3D orientation;
		public Vector3D Orientation {
			get { return orientation; }
			internal set { orientation = value; }
		}
	}
}
