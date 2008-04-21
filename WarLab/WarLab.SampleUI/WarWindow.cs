using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ScientificStudio.Charting;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using VisualListener;
using System.Collections.Specialized;
using WarLab.SampleUI.Charts;
using System.Windows.Threading;

namespace WarLab.SampleUI {
	public class WarWindow : Window {
		public WarWindow() {
			timeControl = world.GetTimeControl();
			world.CollectionChanged += Objects_CollectionChanged;
			world.ObjectDestroyed += world_ObjectDestroyed;
			WindowState = WindowState.Maximized;

			Loaded += OnLoaded;
		}

		private readonly World world = World.Instance;
		public World World {
			get { return world; }
		}

		private readonly List<GraphicalObject> uiGraphs = new List<GraphicalObject>();
		private void AddUIGraph(GraphicalObject graph) {
			plotter.AddGraph(graph);
			uiGraphs.Add(graph);
		}

		private void UpdateUI() {
			foreach (var graph in uiGraphs) {
				(graph as WarGraph).DoUpdate();
			}
		}

		private void world_ObjectDestroyed(object sender, ObjectDestroyedEventArgs e) {
			if (createdGraphs.ContainsKey(e.DestroyedObject)) {
				GraphicalObject graph = createdGraphs[e.DestroyedObject];
				plotter.Children.Remove(graph);
			}
		}

		private readonly Dictionary<WarObject, GraphicalObject> createdGraphs = new Dictionary<WarObject, GraphicalObject>();

		private void Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:

					WarObject warObj = (WarObject)e.NewItems[0];
					GraphicalObject graph = Renderers.CreateGraphForWarObject(warObj);
					createdGraphs.Add(warObj, graph);
					AddUIGraph(graph);

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

		private Button timeBtn;
		private ChartPlotter plotter = new ChartPlotter();
		protected ChartPlotter Plotter {
			get { return plotter; }
		}
		private DispatcherTimer dispTimer = new DispatcherTimer();
		private void OnLoaded(object sender, RoutedEventArgs e) {
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			plotter.Viewport.Visible = new Rect(0, 0, 1000, 1000);
			plotter.Viewport.Margin = new Thickness(30);
			plotter.AddGraph(new Axises());
			AddUIGraph(new ClockGraph());

			grid.Children.Add(plotter);

			Border border = new Border
			{
				BorderBrush = (Brush)Application.Current.Resources["logBrush"],
				BorderThickness = new Thickness(3),
			};
			Grid.SetColumn(border, 1);

			DockPanel dockPanel = new DockPanel();
			Slider slider = new Slider
			{
				Minimum = 0,
				Maximum = 10,
				Value = 1,
				AutoToolTipPlacement = AutoToolTipPlacement.BottomRight,
				TickPlacement = TickPlacement.BottomRight
			};
			slider.ValueChanged += slider_ValueChanged;
			DockPanel.SetDock(slider, Dock.Top);

			timeBtn = new Button { Content = "Pause", Margin = new Thickness(0, 2, 0, 2) };
			DockPanel.SetDock(timeBtn, Dock.Top);
			timeBtn.Click += button_Click;

			TextBlock textBlock = new TextBlock
			{
				Text = "Log",
				TextAlignment = TextAlignment.Center,
				Background = Brushes.SeaShell
			};
			DockPanel.SetDock(textBlock, Dock.Top);

			VisualListenerControl visualListener = new VisualListenerControl();
			DockPanel.SetDock(visualListener, Dock.Bottom);

			dockPanel.Children.Add(slider);
			dockPanel.Children.Add(timeBtn);
			dockPanel.Children.Add(textBlock);
			dockPanel.Children.Add(visualListener);

			border.Child = dockPanel;
			grid.Children.Add(border);

			Content = grid;

			OnLoadedCore();
			dispTimer.Tick += dispTimer_Tick;
			dispTimer.Interval = TimeSpan.FromMilliseconds(5);
			dispTimer.Start();
		}

		private void dispTimer_Tick(object sender, EventArgs e) {
			Tick();
		}

		private void Tick() {
			world.Update();
			UpdateUI();
		}

		ITimeControl timeControl;
		private void button_Click(object sender, RoutedEventArgs e) {
			if (timeControl.IsRunning) {
				timeControl.Stop();
				timeBtn.Content = "Resume";
			}
			else {
				timeControl.Start();
				timeBtn.Content = "Pause";
			}
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			timeControl.Speed = e.NewValue;
		}

		protected virtual void OnLoadedCore() { }
	}
}
