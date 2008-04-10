using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Path {
	public struct Position {
		public Position(Vector3D point, Vector3D orientation) {
			this.point = point;
			this.orientation = orientation;
		}

		private Vector3D point;
		public Vector3D Point {
			get { return point; }
			set { point = value; }
		}

		private Vector3D orientation;
		public Vector3D Orientation {
			get { return orientation; }
			set { orientation = value; }
		}
	}
}
