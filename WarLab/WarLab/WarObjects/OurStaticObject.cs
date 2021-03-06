﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public abstract class OurStaticObject : StaticObject, IHasImportance {
		#region IHasImportance Members

		private double importance = 1;
		public double Importance {
			get { return importance; }
			set {
				Verify.IsNonNegative(value);

				importance = value;
			}
		}

		#endregion
	}
}
