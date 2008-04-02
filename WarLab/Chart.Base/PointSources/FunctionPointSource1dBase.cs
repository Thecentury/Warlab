using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;

namespace ScientificStudio.Charting.PointSources {
	public abstract class FunctionPointSource1dBase : PointSource1dBase {

		public double Start { get; set; }
		public double Duration { get; set; }
		public int Number { get; set; }

	}
}
