using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public class TargetInfo {
		public TargetInfo(int id, Vector3D position) {
			this.ID = id;
			this.Position = position;
		}

		public int ID;
		public Vector3D Position;
	}
}
