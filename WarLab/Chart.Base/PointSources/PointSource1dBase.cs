using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public abstract class PointSource1dBase : Animatable, IPointSource {
		#region IPointSource Members

		public ICollection<Point> GetPoints() {
			Rect bounds = Bounds;
			var pts = GetPointsCore();
			if (bounds != Bounds) {
				RaiseBoundsChanged();
			}

			return pts;
		}

		protected abstract ICollection<Point> GetPointsCore();

		public abstract Rect Bounds { get; }

		public event EventHandler BoundsChanged;

		protected void RaiseBoundsChanged() {
			if (BoundsChanged != null) {
				BoundsChanged(this, EventArgs.Empty);
			}
		}

		public event EventHandler PointsChanged;
		protected void RaisePointsChanged() {
			if (PointsChanged != null) {
				PointsChanged(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
