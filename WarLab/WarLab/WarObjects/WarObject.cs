using System;
using System.Collections.Generic;
using System.Text;
using WarLab.AI;
using System.Windows.Markup;
using System.Windows;
using System.ComponentModel;

namespace WarLab {
	/// <summary>
	/// Базовый класс для любого объекта мира.
	/// </summary>
	public abstract class WarObject {
		private readonly int index;
		public WarObject() {
			Type type = GetType();
			if (!counters.ContainsKey(type)) {
				counters[type] = 1;
				index = 1;
			}
			else {
				counters[type]++;
				index = counters[type];
			}
		}

		private World world;
		[Browsable(false)]
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
			set { position = value; }
		}

		internal void UpdateAI(WarTime time) {
			ai.Update(time);
		}

		/// <summary>
		/// Updates this instance.
		/// </summary>
		internal void UpdateSelf(WarTime time) {
			UpdateImpl(time);
		}

		protected virtual void UpdateImpl(WarTime time) { }

		private WarAI ai = null;

		public WarAI AI {
			get { return ai; }
		}

		internal void SetAI(WarAI ai) {
			this.ai = ai;
			ai.AttachControlledObject(this);
		}

		internal void ExecuteAICommands() {
			ai.ExecuteCommands();
		}

		protected internal virtual void OnAddedToWorld() { }

		protected internal virtual void OnRemovedFromWorld() { }

		private static readonly Dictionary<Type, int> counters = new Dictionary<Type, int>();

		public string Name {
			get { return NameCore + " #" + index; }
		}

		public sealed override string ToString() {
			return Name;
		}

		protected virtual string NameCore {
			get { return GetType().Name; }
		}
	}
}
