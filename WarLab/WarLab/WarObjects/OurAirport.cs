using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class OurAirport : Airport, IHasImportance {

		protected override bool VerifyPlaneTypeCore(Plane plane) {
			return plane is OurFighter;
		}

		protected override string NameCore {
			get {
				return "Аэродром обороны";
			}
		}

		private int numOfChannels = Default.OurAirportNumOfChannels;
		public int NumOfChannels {
			get { return numOfChannels; }
			set { numOfChannels = value; }
		}

		protected override bool CanLaunchCore<T>() {
			return Planes.Count(pi => pi.State == AirportPlaneState.InAir) < numOfChannels;
		}

		#region IHasImportance Members

		private double importance = 1;
		public double Importance {
			get { return importance; }
			set {
				Verify.IsPositive(value);
				importance = value;
			}
		}

		#endregion
	}
}
