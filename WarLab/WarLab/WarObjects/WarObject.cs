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

		internal void UpdateAI(WarTime time) {
			ai.Update(time);
		}

		/// <summary>
		/// Updates this instance.
		/// </summary>
		internal void UpdateSelf(WarTime warTime) {
			UpdateImpl(warTime);
		}

		protected virtual void UpdateImpl(WarTime warTime) { }

		private WarAI ai = null;

		protected WarAI AI {
			get { return ai; }
		}

		internal void SetAI(WarAI ai) {
			this.ai = ai;
			ai.Attach(this);
		}

		internal void ExecuteAICommands() {
			ai.ExecuteCommands();
		}
	}
}
