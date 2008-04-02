using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Animation;

namespace ScientificStudio.Charting.PointSources {
	public class FunctionPointSource1d : FunctionPointSource1dBase {

		public Func<double, double> F { get; set; }

		// todo сделать нормальное увеличение версии при изменении свойства
		private int version = 1;
		private int cachedVersion = 0;
		private List<Point> cachedPoints;
		protected override ICollection<Point> GetPointsCore() {
			if (cachedVersion == version) {
				return cachedPoints;
			}
			cachedVersion = version;

			cachedPoints = new List<Point>(Number);

			double duration = Duration;
			double start = Start;
			int number = Number;

			double yMin = Double.PositiveInfinity;
			double yMax = Double.NegativeInfinity;

			double xMin = start;
			double xMax = start + duration;

			double step = duration / (number);
			for (int i = 0; i < number + 1; i++) {
				double x = start + step * i;
				double y = F(x);
				
				yMin = Math.Min(yMin, y);
				yMax = Math.Max(yMax, y);

				cachedPoints.Add(new Point(x, y));
			}

			bounds = new Rect(new Point(xMin, yMin), new Point(xMax, yMax));

			return cachedPoints;
		}

		protected override Freezable CreateInstanceCore() {
			return new FunctionPointSource1d();
		}

		private Rect bounds = Rect.Empty;
		public override Rect Bounds {
			get { return bounds; }
		}
	}
}
