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
			// объект для управления временем мира.
			timeControl = world.GetTimeControl();

			// событие изменения коллекции объектов мира
			world.CollectionChanged += Objects_CollectionChanged;
			// событие уничтожения какого-либо из объектов
			world.ObjectDestroyed += world_ObjectDestroyed;
			WindowState = WindowState.Maximized;

			Loaded += OnLoaded;
		}

		private readonly World world = World.Instance;
		/// <summary>
		/// Военный мир, который показывается.
		/// </summary>
		public World World {
			get { return world; }
		}

		/// <summary>
		/// Список добавленных объектов для отображения объектов мира - нужен потому,
		/// что есть служебные графики, не рисующие объеты мира.
		/// </summary>
		private readonly List<GraphicalObject> uiGraphs = new List<GraphicalObject>();
		private void AddUIGraph(GraphicalObject graph) {
			plotter.AddGraph(graph);
			uiGraphs.Add(graph);
		}

		/// <summary>
		/// Выполнить перерисовку.
		/// </summary>
		private void UpdateUI() {
			foreach (var graph in uiGraphs) {
				(graph as WarGraph).DoUpdate();
			}
		}

		private void world_ObjectDestroyed(object sender, ObjectDestroyedEventArgs e) {
			// удаляем график, отображавший удаленный объект мира.
			if (createdGraphs.ContainsKey(e.DestroyedObject)) {
				GraphicalObject graph = createdGraphs[e.DestroyedObject];
				RemoveGraph(e.DestroyedObject);

				uiGraphs.Remove(graph);

				plotter.Children.Remove(graph);
			}
			else { }
		}

		/// <summary>
		/// Словарь созданных графиков.
		/// Ключ - объект мира, значение - сам график.
		/// </summary>
		private readonly Dictionary<WarObject, GraphicalObject> createdGraphs = new Dictionary<WarObject, GraphicalObject>();
		private void RemoveGraph(WarObject warObject) {
			createdGraphs.Remove(warObject);
		}

		private void AddGraph(WarObject warObject, GraphicalObject graph) {
			createdGraphs[warObject] = graph;
		}

		private void Objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			WarObject warObj;
			GraphicalObject graph;

			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					// добавить новый график для свежедобавленного объекта мира.
					warObj = (WarObject)e.NewItems[0];
					graph = Renderers.CreateGraphForWarObject(warObj);
					AddGraph(warObj, graph);
					AddUIGraph(graph);

					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Remove:

					// удалить график для удаленного объекта мира.
					warObj = (WarObject)e.OldItems[0];
					if (createdGraphs.ContainsKey(warObj)) {
						graph = createdGraphs[warObj];
						RemoveGraph(warObj);
						uiGraphs.Remove(graph);
						plotter.Children.Remove(graph);
					}
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
		/// <summary>
		/// Таймер, по тику которого происходит обновление мира и перерисовка.
		/// </summary>
		private DispatcherTimer dispTimer = new DispatcherTimer();

		/// <summary>
		/// Создание внутренностей окна - грида, кнопки, графического представления мира и т.п.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoaded(object sender, RoutedEventArgs e) {
			Grid grid = new Grid();
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(4, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			plotter.Viewport.Visible = new Rect(0, 0, 1000, 1000);
			plotter.Viewport.Margin = new Thickness(30);
			plotter.AddGraph(new Axises());
			plotter.AddGraph(new CameraGraph());
			AddUIGraph(new ClockGraph());

			grid.Children.Add(plotter);

			GridSplitter splitter = new GridSplitter
			{
				Background = Brushes.LightGray,
				Width = 4
			};
			//grid.Children.Add(splitter);
			Grid.SetColumn(splitter, 1);

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

			PropertyInspectorControl inspector = new PropertyInspectorControl();
			DockPanel.SetDock(inspector, Dock.Top);

			dockPanel.Children.Add(slider);
			dockPanel.Children.Add(timeBtn);
			dockPanel.Children.Add(inspector);
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

		/// <summary>
		/// Обработчик тика таймера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void dispTimer_Tick(object sender, EventArgs e) {
			Tick();
		}

		/// <summary>
		/// Обновление мира и его визуального представления.
		/// </summary>
		private void Tick() {
			world.Update();
			UpdateUI();
		}

		ITimeControl timeControl;
		/// <summary>
		/// Обработчик нажатия на кнопку "Пауза/Возобновление".
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Метод, вызываемый при загрузке окна.
		/// В нем можно добавлять различные объекты в мир.
		/// </summary>
		protected virtual void OnLoadedCore() { }
	}
}
