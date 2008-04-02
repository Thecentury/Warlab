using System.Collections.Generic;
using System.Windows;
using System;

namespace ScientificStudio.Charting.PointSources {
	
	public interface IPointSource {
		ICollection<Point> GetPoints();
		event EventHandler PointsChanged;

		Rect Bounds { get; }
		event EventHandler BoundsChanged;
	}
}
