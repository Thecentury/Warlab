using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects.Filters {
	public sealed class InclinationFilter : IFilter {
		
		private double criticalAngle = 178;
		public double CriticalAngle {
			get { return criticalAngle; }
			set { criticalAngle = value; }
		}

		#region IFilter Members

		public List<Point> Filter(List<Point> initialPoints) {
			List<Point> res = new List<Point>();
			res.Add(initialPoints[0]);
			int i = 1;
			while (i < initialPoints.Count) {
				bool added = false;
				int j = i;
				while (!added && (j < initialPoints.Count - 1)) {
					Point x1 = res[res.Count - 1];
					Point x2 = initialPoints[j];
					Point x3 = initialPoints[j + 1];

					double a = (x1 - x2).Length;
					double b = (x2 - x3).Length;
					double c = (x1 - x3).Length;

					double angle13 = Math.Acos((a * a + b * b - c * c) / (2 * a * b));
					double degrees = 180 / Math.PI * angle13;
					if (degrees < criticalAngle) {
						res.Add(x2);
						added = true;
						i = j + 1;
					}
					else {
						j++;
					}
				}
				// reached the end of resultPoints
				if (!added) {
					res.Add(initialPoints.GetLast());
					break;
				}
			}
			return res;
		}

		#endregion
	}
}
