using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого объекта мира.
	/// </summary>
	public abstract class WarObject {
		private Vector3D position;
		/// <summary>
		/// Gets or sets the position.
		/// </summary>
		/// <value>The position.</value>
		public Vector3D Position {
			get { return position; }
			internal set { position = value; }
		}

		/// <summary>
		/// Updates this instance.
		/// </summary>
		public abstract void Update(WarTime warTime);
	}
}
