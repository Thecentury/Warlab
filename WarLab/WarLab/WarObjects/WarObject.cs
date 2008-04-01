using System;
using System.Collections.Generic;
using System.Text;
using WarLab.AI;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого объекта мира.
	/// </summary>
	public abstract class WarObject {
		private Vector3D position;
		/// <summary>
		/// Положение объекта в мире.
		/// </summary>
		public Vector3D Position {
			get { return position; }
			internal set { position = value; }
		}


		/// <summary>
		/// Updates this instance.
		/// </summary>
		public void Update(WarTime warTime) {
			ai.Update(warTime);

			UpdateImpl(warTime);
		}

		protected virtual void UpdateImpl(WarTime warTime) { }

		private WarAI ai = null;
		internal void SetAI(WarAI ai) {
			this.ai = ai;
			ai.Attach(this);
		}

		internal void ExecuteAICommands() {
			ai.ExecuteCommands();
		}
	}
}
