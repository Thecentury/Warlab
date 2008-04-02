using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ScientificStudio.Charting.GraphicalObjects;
using System.ComponentModel;
using System.Collections;

namespace ScientificStudio.Charting.Layers {

	public class LayerDeletedEventArgs : EventArgs {
		private readonly GraphicalObject deletedGraph;
		public GraphicalObject DeletedGraph {
			get { return deletedGraph; }
		}

		public LayerDeletedEventArgs(GraphicalObject deletedGraph) {
			this.deletedGraph = deletedGraph;
		}
	}

	public delegate void LayerDeletedHandler(object sender, LayerDeletedEventArgs e);

	/// <summary>
	/// Interaction logic for LevelControl.xaml
	/// </summary>
	public partial class LayerControl : UserControl {
		public LayerControl() {
			InitializeComponent();
			InitializeCommands();
		}

		#region Commands

		#region Inc ZIndex

		private CommandBinding incZIndexCommandBinding;
		public CommandBinding IncZIndexCommandBinding {
			get { return incZIndexCommandBinding; }
		}

		private void incZIndexExecute(object target, ExecutedRoutedEventArgs e) {
			int index = list.SelectedIndex;
			ExchangeLevels(index, index + 1);
			e.Handled = true;
		}

		private void incZIndexCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = Layers != null && list.SelectedItem != null && (list.SelectedIndex < (Layers.Count - 1));
		}

		#endregion

		#region Dec ZIndex

		private CommandBinding decZIndexCommandBinding;
		public CommandBinding DecZIndexCommandBinding {
			get { return decZIndexCommandBinding; }
		}

		private void decZIndexExecute(object target, ExecutedRoutedEventArgs e) {
			int index = list.SelectedIndex;
			// we can move up
			ExchangeLevels(index, index - 1);
			e.Handled = true;
		}

		private void decZIndexCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = Layers != null && list.SelectedItem != null && list.SelectedIndex > 0;
		}

		#endregion

		#region Delete command

		public event LayerDeletedHandler LayerDeleted;
		private void RaiseLayerDeletedEvent(GraphicalObject deletedGraph) {
			if (LayerDeleted != null) {
				LayerDeleted(this, new LayerDeletedEventArgs(deletedGraph));
			}
		}

		private CommandBinding deleteCommandBinding;
		public CommandBinding DeleteCommandBinding {
			get { return deleteCommandBinding; }
		}

		private void deleteExecute(object target, ExecutedRoutedEventArgs e) {
			GraphicalObject graph = ((Layer)list.SelectedValue).Graph;

			Layers.Collection.Remove(graph);
			Layers.Update();
			UpdateView();

			RaiseLayerDeletedEvent(graph);
			e.Handled = true;
		}

		private void deleteCanExecute(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = Layers != null && list.SelectedItem != null;
		}

		#endregion

		private void InitializeCommands() {
			incZIndexCommandBinding = new CommandBinding(
				NavigationCommands.NextPage,
				incZIndexExecute,
				incZIndexCanExecute);
			CommandBindings.Add(incZIndexCommandBinding);

			decZIndexCommandBinding = new CommandBinding(
				NavigationCommands.PreviousPage,
				decZIndexExecute,
				decZIndexCanExecute);
			CommandBindings.Add(decZIndexCommandBinding);

			deleteCommandBinding = new CommandBinding(
				ApplicationCommands.Delete,
				deleteExecute,
				deleteCanExecute);
			CommandBindings.Add(deleteCommandBinding);
		}

		#endregion

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public LayersCollection Layers {
			get { return (LayersCollection)GetValue(LayersProperty); }
			set { SetValue(LayersProperty, value); }
		}

		public static readonly DependencyProperty LayersProperty =
			DependencyProperty.Register(
			  "Layers",
			  typeof(LayersCollection),
			  typeof(LayerControl),
			  new FrameworkPropertyMetadata(null));

		public void Update() {
			Layers.Update();
			UpdateView();
		}

		private void UpdateView() {
			ICollectionView dataView =
				CollectionViewSource.GetDefaultView(list.ItemsSource);
			dataView.Refresh();
		}

		private void ExchangeLevels(int i1, int i2) {
			GraphicalObject g1 = Layers[i1].Graph;
			GraphicalObject g2 = Layers[i2].Graph;

			int z1 = Panel.GetZIndex(g1);
			int z2 = Panel.GetZIndex(g2);

			Panel.SetZIndex(g1, z2);
			Panel.SetZIndex(g2, z1);

			Layer p1 = Layers[i1];
			Layer p2 = Layers[i2];

			Layers[i1] = p2;
			Layers[i2] = p1;

			UpdateView();

			list.SelectedIndex = i2;
		}
	}
}
