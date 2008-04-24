using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	public abstract class PlaneAI : DynamicObjectAI {
		public Plane ControlledPlane {
			get { return ControlledObject as Plane; }
		}

		public Vector3D AirportPosition {
			get { return ControlledPlane.AirportPosition; }
		}
	}
}
