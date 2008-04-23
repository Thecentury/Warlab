using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class AirportPlaneInfo {
		private readonly Plane plane;
		public Plane Plane {
			get { return plane; }
		} 

		private AirportPlaneState state = AirportPlaneState.ReadyToFly;
		public AirportPlaneState State {
			get { return state; }
			set { state = value; }
		}

		public AirportPlaneInfo(Plane plane) {
			if (plane == null)
				throw new ArgumentNullException("plane");
	
			this.plane = plane;
		}
	}
}
