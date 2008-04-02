using System.Windows;
using System.Windows.Controls;
using System.Collections.Specialized;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;
using System.Windows.Media;

namespace ScientificStudio.Charting.GraphicalObjects {
	/// <summary>
	/// Interaction logic for Legend.xaml
	/// </summary>
	public partial class Legend : ContentGraph {
		public Legend() {
			InitializeComponent();
			Canvas.SetTop(this, 5);
			Canvas.SetRight(this, 5);
		}

		public void AddLegendItem(LegendItem legendItem) {
			stackPanel.Children.Add(legendItem);
		}

		protected override void OnSetViewport(Viewport2D viewport) {
			AttachPlotter();
		}

		protected override void OnDetachViewport() {
			DetachPlotter();
		}

		protected override void OnViewportChanged() {
			PopulateLegend();
		}

		private void AttachPlotter() {
			if (ParentChartPlotter != null) {
				ParentChartPlotter.CollectionChanged += ParentChartPlotter_CollectionChanged;
			}
		}

		private void DetachPlotter() {
			if (ParentChartPlotter != null) {
				ParentChartPlotter.CollectionChanged -= ParentChartPlotter_CollectionChanged;
			}
		}

		private readonly Dictionary<GraphicalObject, LegendItem> cachedLegendItems = new Dictionary<GraphicalObject, LegendItem>();

		private void ParentChartPlotter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			// todo сделать полный анализ
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					GraphicalObject graph = e.NewItems[0] as GraphicalObject;
					if (graph != null && Legend.GetShowInLegend(graph)) {
						graph.PropertyChanged += graph_PropertyChanged;
						
						LegendItem legendItem = graph.Description.LegendItem;
						cachedLegendItems.Add(graph, legendItem);
						AddLegendItem(legendItem);
					}
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Remove:
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					break;
			}
		}

		private void graph_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (e.PropertyName == "Description") {
				GraphicalObject graph = (GraphicalObject)sender;
				LegendItem oldLegendItem = cachedLegendItems[graph];
				int index = stackPanel.Children.IndexOf(oldLegendItem);
				stackPanel.Children.RemoveAt(index);

				LegendItem newLegendItem = graph.Description.LegendItem;
				cachedLegendItems[graph] = newLegendItem;
				stackPanel.Children.Insert(index, newLegendItem);
			}
		}

		private PopulationMethod populationMethod = PopulationMethod.Auto;
		public PopulationMethod PopulationMethod {
			get { return populationMethod; }
			set {
				if (populationMethod != value) {
					populationMethod = value;
					// todo доделать
					PopulateLegend();
				}
			}
		}

		private void PopulateLegend() {
			if (populationMethod == PopulationMethod.Auto) {
				// todo доделать
				foreach (IGraphicalObject graph in ParentChartPlotter.GraphChildren) {
					// todo подписываться на события об изменении коллекции у plotter'а
					GraphicalObject d = graph as GraphicalObject;
					if (d != null && GetShowInLegend(d)) {
						LegendItem legendItem = d.Description.LegendItem;
						AddLegendItem(legendItem);
					}
				}
			}
		}

		#region ShowInLegend attached dependency property

		public static bool GetShowInLegend(DependencyObject obj) {
			return (bool)obj.GetValue(ShowInLegendProperty);
		}

		public static void SetShowInLegend(DependencyObject obj, bool value) {
			obj.SetValue(ShowInLegendProperty, value);
		}

		public static readonly DependencyProperty ShowInLegendProperty =
			DependencyProperty.RegisterAttached(
			"ShowInLegend",
			typeof(bool),
			typeof(Legend), new UIPropertyMetadata(
				false
				));

		#endregion
	}

	public enum PopulationMethod {
		Auto,
		Manual
	}
}
