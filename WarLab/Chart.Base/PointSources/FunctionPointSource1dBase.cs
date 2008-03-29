using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public abstract class FunctionPointSource1dBase : Animatable, IPointSource {

		public double Start { get; set; }
		public double Duration { get; set; }
		public int Number { get; set; }


		#region IPointSource Members

		public List<Point> GetPoints() {
			Rect bounds = Bounds;
			var pts = GetPointsCore();
			if (bounds != Bounds) {
				RaiseBoundsChanged();
			}

			return pts;
		}

		protected abstract List<Point> GetPointsCore();

		public abstract Rect Bounds { get; }
		public event EventHandler BoundsChanged;

		protected void RaiseBoundsChanged() {
			if (BoundsChanged != null) {
				BoundsChanged(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
