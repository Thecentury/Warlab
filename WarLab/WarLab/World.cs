using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	public sealed class World {
		private static readonly World instance = new World();
		public static World Instance {
			get { return instance; }
		} 

	}
}
