using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects.Filters {
	internal interface IFilter {
		List<Point> Filter(List<Point> points);
	}
}
