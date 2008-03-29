using System.Collections.Generic;
using System.Windows;
using System;

namespace ScientificStudio.Charting.PointSources {
	
	public interface IPointSource {
		List<Point> GetPoints();
		Rect Bounds { get; }
		event EventHandler BoundsChanged;
	}
}
