using System;
using System.Collections.Generic;
using System.Text;
using WarLab.AI;
using System.Windows.Markup;
using System.Windows;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого объекта мира.
	/// </summary>
	public abstract class WarObject  {
		private static int global_id = 0;
		private int id = global_id++;

		private World world;
		public World World {
			get { return world; }
			internal set { world = value; }
		}

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

		public WarAI AI {
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
