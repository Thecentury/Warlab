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
using WarLab.WarObjects;
using WarLab.AI;

namespace WarLab.SampleUI {

	public delegate GraphicalObject RendererCreator(WarObject warObj);

	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			timeControl = world.GetTimeControl();

			InitializeComponent();
	
			world.CollectionChanged += Objects_CollectionChanged;
			world.ObjectDestroyed += world_ObjectDestroyed;

			Loaded += MainWindow_Loaded;
		}

		private void world_ObjectDestroyed(object sender, ObjectDestroyedEventArgs e) {
			if (createdGraphs.ContainsKey(e.DestroyedObject)) {
				GraphicalObject graph = createdGraphs[e.DestroyedObject];
				plotter.Children.Remove(graph);
			}
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			ClockGraph clock = new ClockGraph();
			uiGraphs.Add(clock);
			plotter.AddGraph(clock);

			//world.RegisterAIForWarObject<SamplePlaneAI, SamplePlane>();
			world.RegisterAIForWarObject<PathDrivenAI, SamplePlane>();
			//world.RegisterAIForWarObject<SamplePlaneAI, SampleEnemyPlane>();
			world.RegisterAIForWarObject<PathDrivenAI, SampleEnemyPlane>();
			world.RegisterAIForWarObject<ZRKAI, SimpleZRK>();

			SampleEnemyPlane plane = null;
			for (int i = 0; i < 3; i++) {
				double x = StaticRandom.NextDouble() * 500 + 250;
				double y = StaticRandom.NextDouble() * 500 + 250;
				world.AddWarObject(plane = new SampleEnemyPlane(), new Vector3D(x, y, 1));
			}

			Rocket rocket = new Rocket
			{
				TargetPoint = new Vector3D(1000, 1000, 0),
				TimeOfExposion = TimeSpan.FromSeconds(8),
				Speed = Speed.FromKilometresPerHour(200),
				DamageRange = Distance.FromMetres(200),
				Damage = 10
			};

			//world.AddWarObject(rocket, new Vector3D());
			//((ImprovedRocketAI)rocket.AI).Target = plane;

			world.AddWarObject(new RLS(), new Vector3D());
			world.AddWarObject(new SimpleZRK(), new Vector3D(30, 100));

			dispTimer.Tick += dispTimer_Tick;
			dispTimer.Interval = TimeSpan.FromMilliseconds(5);
			dispTimer.Start();
		}

		private void dispTimer_Tick(object sender, EventArgs e) {
			Tick();
		}

		DispatcherTimer dispTimer = new DispatcherTimer();

		World world = World.Instance;

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

		private void Tick() {
			world.Update();
			UpdateUI();
		}

		ITimeControl timeControl;
		private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
			timeControl.Speed = e.NewValue;
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e) {
			if (timeControl.IsRunning) {
				timeControl.Stop();
				timeBtn.Content = "Resume";
			}
			else {
				timeControl.Start();
				timeBtn.Content = "Pause";
			}
		}
	}
}
