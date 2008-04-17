using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public delegate void ObjectDestroyedEventHandler(object sender, ObjectDestroyedEventArgs e);
	public sealed class ObjectDestroyedEventArgs : EventArgs {
		public ObjectDestroyedEventArgs(WarObject destroyedObject) {
			this.destroyedObject = destroyedObject;
		}

		private readonly WarObject destroyedObject;
		public WarObject DestroyedObject {
			get { return destroyedObject; }
		}
	}
}
