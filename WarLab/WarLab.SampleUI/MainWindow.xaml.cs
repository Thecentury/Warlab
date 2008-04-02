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


		DateTime startTime;
		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
			world.RegisterAIForWarObject<SamplePlaneAI, SamplePlane>();

			for (int i = 0; i < 100; i++) {
				double x = StaticRandom.NextDouble() * 500 + 250;
				double y = StaticRandom.NextDouble() * 500 + 250;
				double h = 1;

				world.AddWarObject(new SamplePlane(), new Vector3D(x, y, h));
			}

			startTime = DateTime.Now;
			AnimationTimeline.SetDesiredFrameRate(animation, 100);

			AnimationClock clock = animation.CreateClock();
#if !true
			ApplyAnimationClock(TimeProperty, clock);
#else
			timer = new Timer(OnTimerTick, null, 0, 10);
#endif
		}

		private Timer timer = null;

		private delegate void MethodInvoker();
		private void OnTimerTick(object o) {
			Dispatcher.Invoke(DispatcherPriority.Normal, (MethodInvoker)(() => Tick()));
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
				(graph as SpriteGraph).DoUpdate();
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
