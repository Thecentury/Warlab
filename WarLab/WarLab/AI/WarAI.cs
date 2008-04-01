﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public abstract class WarAI {
		private WarObject controlledObject;
		internal WarObject ControlledObject {
			get { return controlledObject; }
		}

		internal void Attach(WarObject controlledObject) {
			this.controlledObject = controlledObject;
			AttachCore(controlledObject);
		}

		protected virtual void AttachCore(WarObject controlledObject) { }

		private readonly Dictionary<Type, IAICommand> commands = new Dictionary<Type, IAICommand>();

		public abstract void Update(WarTime time);

		protected void AddCommand(IAICommand command) {
			commands[command.GetType()] = command;
		}

		internal void ExecuteCommands() {
			foreach (IAICommand command in commands.Values) {
				command.Execute();
			}
			commands.Clear();
		}
	}
}
