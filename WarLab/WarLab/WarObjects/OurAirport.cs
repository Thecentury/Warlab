﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class OurAirport : Airport, IHasImportance {

		protected override bool VerifyPlaneTypeCore(Plane plane) {
			return plane is OurFighter;
		}

		#region IHasImportance Members

		private double importance = 1;
		public double Importance {
			get { return importance; }
			set { importance = value; }
		}

		#endregion
	}
}
