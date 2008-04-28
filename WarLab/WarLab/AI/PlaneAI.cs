using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WarLab.AI {
	public abstract class PlaneAI : DynamicObjectAI {
		[Browsable(false)]
		public Plane ControlledPlane {
			get { return ControlledObject as Plane; }
		}

		[Browsable(false)]
		public Vector3D AirportPosition {
			get { return ControlledPlane.AirportPosition; }
		}

		protected void LandPlane() {
			ControlledPlane.Airport.LandPlane(ControlledPlane);
		}

		protected void ReloadAndRefuelInAir() {
			ControlledPlane.Airport.ReloadAndRefuelInAir(ControlledPlane);
		}
	}
}
