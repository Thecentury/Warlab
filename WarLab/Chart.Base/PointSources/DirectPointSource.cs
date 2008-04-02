using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using ScientificStudio.Charting.Auxilliary;

namespace ScientificStudio.Charting.PointSources {
	public class DirectPointSource : PointSource1dBase {
		
		#region Ctors

		public DirectPointSource() { }
		public DirectPointSource(Point[] points) {
			if (points == null)
				throw new ArgumentNullException("points");

			this.points = points;
		}

		public DirectPointSource(double[] x, double[] y) {
			if (x == null)
				throw new ArgumentNullException("x");
			if (y == null)
				throw new ArgumentNullException("y");
			if (x.Length != y.Length)
				throw new ArgumentException("Lengths of both arguments should be equal");

			int length = x.Length;
			points = new Point[length];
			for (int i = 0; i < length; i++) {
				points[i] = new Point(x[i], y[i]);
			}
		}

		#endregion

		private Point[] points = new Point[0];
		public Point[] PointsArray {
			get { return points; }
			set {
				if (value == null)
					throw new ArgumentNullException("value");
	
				if (points != value) {
					points = value;
					RaisePointsChanged();
				}
			}
		}

		#region IPointSource Members

		protected override ICollection<Point> GetPointsCore() {
			bounds = points.GetBounds();

			return points;
		}

		protected override Freezable CreateInstanceCore() {
			return new DirectPointSource();
		}

		private Rect bounds = Rect.Empty;
		public override Rect Bounds {
			get { return bounds; }
		}

		#endregion
	}
}
