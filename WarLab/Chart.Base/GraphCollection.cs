using System;
using System.Windows.Controls;
using System.Windows;
using ScientificStudio.Charting.GraphicalObjects;
using System.Collections.Specialized;

namespace ScientificStudio.Charting {
	internal class GraphCollection : UIElementCollection, INotifyCollectionChanged {
		private readonly ChartPlotter plotter;
		public GraphCollection(ChartPlotter visualParent, FrameworkElement logicalParent)
			: base(visualParent, logicalParent) {
			plotter = visualParent;
		}

		private void TryAttach(UIElement element) {
			IGraphicalObject graph = element as IGraphicalObject;
			if (graph != null) {
				plotter.AttachChild(graph);
			}
		}

		private void TryDetach(UIElement element) {
			IGraphicalObject graph = element as IGraphicalObject;
			if (graph != null) {
				plotter.DetachGraph(graph);
			}
		}

		public override int Add(UIElement element) {
			TryAttach(element);
			return base.Add(element);
		}

		public override void Insert(int index, UIElement element) {
			TryAttach(element);
			base.Insert(index, element);
		}

		public override void Clear() {
			foreach (UIElement elem in this) {
				TryDetach(elem);
			}
			base.Clear();
		}

		public override void Remove(UIElement element) {
			TryDetach(element);
			base.Remove(element);
		}

		public override void RemoveAt(int index) {
			TryDetach(base[index]);
			base.RemoveAt(index);
		}

		public override UIElement this[int index] {
			get { return base[index]; }
			set {
				TryDetach(base[index]);
				base[index] = value;
				TryAttach(value);
			}
		}

        #region INotifyCollectionChanged Members

        // todo implement this interface
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}
