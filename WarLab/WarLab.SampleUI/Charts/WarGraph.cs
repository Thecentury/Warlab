using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;

namespace WarLab.SampleUI.Charts {
	public abstract class WarGraph : GraphicalObject {
		public void DoUpdate() {
			MakeDirty();
			InvalidateVisual();
		}
	}
}
