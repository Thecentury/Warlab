using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;

namespace ScientificStudio.Charting.GraphicalObjects.Descriptions
{
    public sealed class PenDescription : StandartDescription
    {
        protected override LegendItem CreateLegendItemCore()
        {
            return new LineLegendItem(this);
        }

        protected override void AttachCore(IGraphicalObject graph)
        {
            base.AttachCore(graph);
            PointsGraph g = graph as PointsGraph;
            if (g == null)
            {
                throw new ArgumentException("Pen description can only be attached to PointsGraph", "graph");
            }
            pen = g.LinePen;
        }

        private Pen pen = null;
        public Pen Pen
        {
            get { return pen; }
        }
    }
}
