using System.Collections.Generic;
using System.Windows;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.Auxilliary.Interpolators {
	public interface IGridInterpolator {
		void Load(IWarpedGrid2d grid);
		IEnumerable<object> Points(Rect bounds);
	}
}
