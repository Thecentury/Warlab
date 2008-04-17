using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;

namespace WarLab.SampleUI.Charts {
	public abstract class WarGraph : GraphicalObject {
		private WarTime time;
		protected WarTime Time { get { return time; } }

		public void DoUpdate(WarTime time) {
			this.time = time;

			MakeDirty();
			InvalidateVisual();
		}
	}
}
