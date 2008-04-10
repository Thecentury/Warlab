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
using EnemyPlanes;

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
		EnemyManager cp;
		StaticTarget target2;
		StaticTarget target1;
		DateTime startTime;
		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			world.RegisterAIForWarObject<EnemyFighterAI, EnemyFighter>();
			world.RegisterAIForWarObject<EnemyBomberAI, EnemyBomber>();
			world.RegisterAIForWarObject<EnemyAirportAI, EnemyAirport>();
			world.RegisterAIForWarObject<StaticTargetAI, StaticTarget>();

			EnemyAirport ba = new EnemyAirport();
			EnemyAirport fa = new EnemyAirport();
			cp = new EnemyManager(ba, fa);
			EnemyBomber bomber = new EnemyBomber(10, 600, 60);
			EnemyFighter fighter1 = new EnemyFighter(10, 300, 140.0);
			EnemyFighter fighter2 = new EnemyFighter(10, 300, 120.0);
			EnemyFighter fighter3 = new EnemyFighter(10, 300, 120.0);
			fa.Planes.Add(fighter1);
			fa.Planes.Add(fighter2);
			fa.Planes.Add(fighter3);
			ba.Planes.Add(bomber);
			world.AddWarObject(ba, new Vector3D(10, 100, 1));
			world.AddWarObject(fa, new Vector3D(10, 500, 1));
			world.AddWarObject(bomber, ba.Position);
			world.AddWarObject(fighter1, fa.Position);
			world.AddWarObject(fighter2, fa.Position);
			world.AddWarObject(fighter3, fa.Position);
			((EnemyBomberAI)bomber.AI).FightersRadius = 60.0;
			((EnemyBomberAI)bomber.AI).targetReached += new EnemyBomberAI.TargetReachedDelegate(MainWindow_targetReached);
			//cp.Bombers.Add(bomber,(EnemyBomberAI)bomber.AI);
			//cp.Fighters.Add(fighter1,(EnemyFighterAI)fighter1.AI);
			//cp.Fighters.Add(fighter2,(EnemyFighterAI)fighter2.AI);
			target1 = new StaticTarget();
			target2 = new StaticTarget();
			world.AddWarObject(target2, new Vector3D(30, 900, 1));
			world.AddWarObject(target1, new Vector3D(650, 250, 1));
			cp.Navigate(bomber, target1);
			cp.Navigate(fighter1, bomber);
			cp.Navigate(fighter2, bomber);
			cp.Navigate(fighter3, bomber);

			startTime = DateTime.Now;

			dispTimer.Tick += dispTimer_Tick;
			dispTimer.IsEnabled = true;
			dispTimer.Interval = TimeSpan.FromMilliseconds(5);
			dispTimer.Start();
		}

		private void dispTimer_Tick(object sender, EventArgs e) {
			Tick();
		}

		void MainWindow_targetReached(Args args) {
			if (args.Target == target1)
				cp.Navigate(args.Bomber, target2);
			if (args.Target == target2)
				cp.Navigate(args.Bomber, target1);
		}

		DispatcherTimer dispTimer = new DispatcherTimer();

		World world = new World();

		private readonly List<GraphicalObject> uiGraphs = new List<GraphicalObject>();
		private void AddUIGraph(GraphicalObject graph) {
			plotter.AddGraph(graph);
			uiGraphs.Add(graph);
		}

		private void UpdateUI() {
			foreach (var graph in uiGraphs) {
				(graph as SpriteGraph).DoUpdate();
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

		private TimeSpan totalTime = new TimeSpan();
		private TimeSpan tickDelta = TimeSpan.FromMilliseconds(30);

		TimeSpan prevFrameTime = TimeSpan.Zero;
		int counter = 0;
		private void Tick() {
			counter++;
			bool measureDuration = counter % 50 == 0;

			int frameStartTime = Environment.TickCount;

			DateTime now = DateTime.Now;
			TimeSpan totalDelta = now - startTime;
			totalTime = totalTime.Add(tickDelta);
			TimeSpan prevDelta = totalDelta - prevFrameTime;

			//WarTime time = new WarTime(tickDelta, totalTime);
			WarTime time = new WarTime(prevDelta, totalDelta);

			world.Update(time);
			UpdateUI();

			int duration = Environment.TickCount - frameStartTime;
			if (measureDuration) {
				Debug.WriteLine(prevDelta.TotalMilliseconds);
				//Debug.WriteLine(String.Format("Duration = {0} ms", duration));
			}

			prevFrameTime = totalDelta;
		}
	}
}
