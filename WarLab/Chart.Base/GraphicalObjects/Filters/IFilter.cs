using System.Collections.Generic;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects.Filters {
	public interface IFilter {
		List<Point> Filter(List<Point> points);
	}
}
