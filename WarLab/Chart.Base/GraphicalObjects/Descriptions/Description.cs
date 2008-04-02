using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificStudio.Charting.GraphicalObjects.Descriptions
{
    public class ResolveLegendItemEventArgs : EventArgs
    {
        public ResolveLegendItemEventArgs(LegendItem legendItem)
        {
            this.legendItem = legendItem;
        }

        private LegendItem legendItem;
        public LegendItem LegendItem {
            get { return legendItem; }
            set { legendItem = value; }
        }
    }

    public abstract class Description
    {
        protected LegendItem CreateLegendItem()
        {
            LegendItem legendItem = CreateLegendItemCore();
            return RaiseResolveLegendItem(legendItem);
        }

        protected virtual LegendItem CreateLegendItemCore() {
            return null;
        }

        public event EventHandler<ResolveLegendItemEventArgs> ResolveLegendItem;
        private LegendItem RaiseResolveLegendItem(LegendItem uncustomizedLegendItem) {
            if (ResolveLegendItem != null) {
                ResolveLegendItemEventArgs e = new ResolveLegendItemEventArgs(uncustomizedLegendItem);
                ResolveLegendItem(this, e);
                return e.LegendItem;
            }
            else
            {
                return uncustomizedLegendItem;
            }
        }

        private IGraphicalObject graph;
        public IGraphicalObject Graph
        {
            get { return graph; }
        }

        internal void Attach(IGraphicalObject graph)
        {
            this.graph = graph;
            AttachCore(graph);
        }

        protected virtual void AttachCore(IGraphicalObject graph) { }

        internal void Detach()
        {
            graph = null;
        }

        public abstract string Brief { get; }

        public abstract string Full { get; }

        public override string ToString()
        {
            return Brief;
        }
    }
}
