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

namespace WarLab.SampleUI {
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			world.CollectionChanged += Objects_CollectionChanged;

			SamplePlane plane = new SamplePlane();
			world.AddWarObject(plane, new Vector3D(500, 500, 1));

			AddUIGraph(new SpriteGraph
			{
				SpriteSource = plane,
				SpriteImage = new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			});

			Loaded += MainWindow_Loaded;
		}

		private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
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

		private void Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
		}

		private TimeSpan totalTime = new TimeSpan();
		private TimeSpan tickDelta = TimeSpan.FromMilliseconds(30);

		private void Tick() {
			totalTime = totalTime.Add(tickDelta);

			WarTime time = new WarTime(tickDelta, totalTime);

			//Debug.WriteLine(totalTime.TotalMilliseconds);

			world.Update(time);
			UpdateUI();
		}
	}
}
