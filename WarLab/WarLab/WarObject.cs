using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого объекта мира.
	/// </summary>
	public abstract class WarObject {
		private double x;
		/// <summary>
		/// X-координата объекта, метров.
		/// </summary>
		public double X {
			get { return x; }
			set { x = value; }
		}

		private double y;
		/// <summary>
		/// Y-координата объекта, метров.
		/// </summary>
		public double Y {
			get { return y; }
			set { y = value; }
		}

		private double height;
		/// <summary>
		/// Высота, на которой находится объект, метров.
		/// </summary>
		public double Height {
			get { return height; }
			set { height = value; }
		}

		/// <summary>
		/// Updates this instance.
		/// </summary>
		public abstract void Update(WarTime warTime);
	}
}
