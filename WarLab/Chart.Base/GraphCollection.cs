using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

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
			int index = base.Add(element);
			RaiseAddItem(element, index);
			return index;
		}

		public override void Insert(int index, UIElement element) {
			TryAttach(element);
			base.Insert(index, element);
			RaiseAddItem(element, index);
		}

		public override void Clear() {
			foreach (UIElement elem in this) {
				TryDetach(elem);
			}
			base.Clear();
			RaiseCollectionReset();
		}

		public override void Remove(UIElement element) {
			TryDetach(element);
			int index = IndexOf(element);
			base.Remove(element);
			RaiseRemoveItem(element, index);
		}

		public override void RemoveAt(int index) {
			TryDetach(base[index]);
			UIElement removedElement = base[index];
			base.RemoveAt(index);
			RaiseRemoveItem(removedElement, index);
		}

		public override UIElement this[int index] {
			get { return base[index]; }
			set {
				UIElement oldElement = base[index];
				TryDetach(oldElement);
				base[index] = value;
				TryAttach(value);
				RaiseAddItem(value, index);
				RaiseRemoveItem(oldElement, index);
			}
		}

		#region INotifyCollectionChanged Members

		private void RaiseRemoveItem(UIElement removedElement, int index) {
			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Remove, removedElement, index));
		}

		private void RaiseAddItem(UIElement addedElement, int index) {
			RaiseCollectionChanged(
				new NotifyCollectionChangedEventArgs(
					NotifyCollectionChangedAction.Add, addedElement, index));
		}

		private void RaiseCollectionReset() {
			RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}
		// todo implement this interface
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
