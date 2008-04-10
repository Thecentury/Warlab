using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public static class Distance {
		public static double FromKilometres(double kilometres) {
			return kilometres * 1000;
		}

		public static double FromMetres(double metres) {
			return metres;
		}
	}
}
