using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Controls;

namespace WarLab.SampleUI.Charts {
	public abstract class WarGraph : GraphicalObject {
		protected WarObject warObject;

		public void DoUpdate() {
			if (warObject != null) {
				Panel.SetZIndex(this, (int)warObject.Position.H);
			}
			MakeDirty();
			InvalidateVisual();
		}
	}
}
