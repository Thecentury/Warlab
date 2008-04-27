using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Controls;

namespace WarLab.SampleUI.Charts {
	public abstract class WarGraph : GraphicalObject {
		private WarObject warObject;
		public WarObject WarObject {
			get { return warObject; }
			set { warObject = value; }
		}

		public void DoUpdate() {
			if (warObject != null) {
				Panel.SetZIndex(this, (int)warObject.Position.H);
			}
			MakeDirty();
			InvalidateVisual();
		}
	}
}
