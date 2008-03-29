using System;
using System.Windows.Media;

namespace ScientificStudio.Charting.GraphicalObjects.Descriptions {
	public sealed class PenDescription : StandartDescription {
		/// <summary>
		/// Initializes a new instance of the <see cref="PenDescription"/> class.
		/// </summary>
		public PenDescription() { }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="PenDescription"/> class.
		/// </summary>
		/// <param name="description">Custom description.</param>
		public PenDescription(string description) : base(description) { }

		protected override LegendItem CreateLegendItemCore() {
			return new LineLegendItem(this);
		}

		protected override void AttachCore(IGraphicalObject graph) {
			base.AttachCore(graph);
			PointsGraph g = graph as PointsGraph;
			if (g == null) {
				throw new ArgumentException("Pen description can only be attached to PointsGraph", "graph");
			}
			pen = g.LinePen;
		}

		private Pen pen = null;
		public Pen Pen {
			get { return pen; }
		}
	}
}
