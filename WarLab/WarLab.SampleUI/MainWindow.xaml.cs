using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using WarLab.SampleUI.WarObjects;
using WarLab.SampleUI.Charts;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;
using WarLab.SampleUI.AI;

namespace WarLab.SampleUI {

	public delegate GraphicalObject RendererCreator(WarObject warObj);

	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			world.CollectionChanged += Objects_CollectionChanged;

			Loaded += MainWindow_Loaded;
		}


		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			world.RegisterAIForWarObject<SamplePlaneAI, SamplePlane>();

			for (int i = 0; i < 30; i++) {
				world.AddWarObject(new SamplePlane(), new Vector3D(500, 500, 1));
			}

			AnimationClock clock = animation.CreateClock();
			
			ApplyAnimationClock(TimeProperty, clock);
		}

		World world = World.Instance;
		EndlessDoubleAnimation animation = new EndlessDoubleAnimation { From = 0, By = 1 };

		private readonly List<GraphicalObject> uiGraphs = new List<GraphicalObject>();
		private void AddUIGraph(GraphicalObject graph) {
			plotter.AddGraph(graph);
			uiGraphs.Add(graph);
		}

		private void UpdateUI() {
			foreach (var graph in uiGraphs) {
				(graph as SpriteGraph).UpdateVisual();
				graph.InvalidateVisual();
			}
		}

		public double Time {
			get { return (double)GetValue(TimeProperty); }
			set { SetValue(TimeProperty, value); }
		}

		public static readonly DependencyProperty TimeProperty =
			DependencyProperty.Register(
			  "Time",
			  typeof(double),
			  typeof(MainWindow),
			  new FrameworkPropertyMetadata(0.0, OnTimeChanged));

		private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MainWindow w = (MainWindow)d;
			w.Tick();
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

		private TimeSpan totalTime = new TimeSpan();
		private TimeSpan tickDelta = TimeSpan.FromMilliseconds(30);

		private void Tick() {
			totalTime = totalTime.Add(tickDelta);

			WarTime time = new WarTime(tickDelta, totalTime);

			world.Update(time);
			UpdateUI();
		}
	}
}
