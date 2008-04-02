using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.Isoline;
using System.Windows;

namespace ScientificStudio.Charting.Auxilliary.Interpolators {
	public interface IGridInterpolator {
		void Load(IWarpedGrid2d grid);
		IEnumerable<object> Points(Rect bounds);
	}
}
