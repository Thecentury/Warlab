using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using ScientificStudio.Charting.GraphicalObjects;
using ScientificStudio.Charting.Layers;
using System.Collections.Specialized;

namespace ScientificStudio.Charting {
	/// <summary>
	/// Control for displaying function graphs.
	/// </summary>
	/// <remarks>
	/// LMB + drag - drag of graphs
	/// RMB + drag - zoom/unzoom
	/// </remarks>
	public class ChartPlotter : Canvas, INotifyCollectionChanged {
		/// <summary>
		/// Initializes a new instance of the <see cref="ChartPlotter"/> class.
		/// </summary>
		public ChartPlotter() {
			InitViewport();

			Background = Brushes.Transparent;
			Focusable = true;
			Loaded += OnLoaded;
		}

		private void InitViewport() {
			Viewport2D v = Viewport;
		}

		private void OnLoaded(object sender, RoutedEventArgs e) {
			Keyboard.Focus(this);
			adornerLayer = AdornerLayer.GetAdornerLayer(this);
			adornerLayer.IsHitTestVisible = false;
		}

		#region Graph children

		public int AddGraph(IGraphicalObject graph) {
			UIElement uiElement = graph as UIElement;
			if (uiElement == null)
				throw new ArgumentException("Graph should be descendant of UIElement class", "graph");

			return Children.Add(uiElement);
		}

		protected override UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent) {
			GraphCollection children = new GraphCollection(this, logicalParent);
			children.CollectionChanged += children_CollectionChanged;
			return children;
		}

		private void children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseCollectionChanged(e);
		}

		protected internal void AttachChild(IGraphicalObject child) {
			base.AddLogicalChild(child);
			child.SetViewport(Viewport);
		}

		protected internal void DetachGraph(IGraphicalObject graph) {
			base.RemoveLogicalChild(graph);
			graph.DetachViewport();
		}

		public IEnumerable<IGraphicalObject> GraphChildren {
			get { return Children.OfType<IGraphicalObject>(); }
		}

		#endregion

		private Viewport2D viewport = null;
		/// <summary>
		/// Gets or sets the viewport.
		/// </summary>
		/// <value>The viewport.</value>
		public Viewport2D Viewport {
			get {
				if (viewport == null) {
					viewport = new Viewport2D();
					AddLogicalChild(viewport);
					viewport.AttachToPlotter(this);
				}
				return viewport;
			}
			set {
				if (value == null)
					throw new ArgumentNullException("value");

				if (viewport != null) {
					viewport.DetachFromPlotter(this);
				}
				RemoveLogicalChild(viewport);
				AddLogicalChild(value);

				viewport = value;
				viewport.AttachToPlotter(this);
				foreach (IGraphicalObject graph in Children.OfType<IGraphicalObject>()) {
					graph.SetViewport(viewport);
				}
			}
		}

		private Rect RenderBounds {
			get { return new Rect(RenderSize); }
		}

		/// <summary>
		/// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
		/// </summary>
		/// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo) {
			base.OnRenderSizeChanged(sizeInfo);
			Viewport.Output = RenderBounds;
			Clip = new RectangleGeometry(RenderBounds);
		}

		#region Mouse & keyboard events

		private DragLock dragLock = DragLock.None;
		private bool shouldKeepRatioWhileZooming = false;
		private bool isZooming = false;
		private bool isDragging = false;
		private Point dragStartPointInVisible;
		private Point zoomStartPoint;
		protected override void OnPreviewMouseDown(MouseButtonEventArgs e) {
			base.OnPreviewMouseDown(e);
			// dragging
			if (e.ChangedButton == MouseButton.Left && !isZooming) {
				Point dragStartPointInOutput = e.GetPosition(this);
				dragStartPointInVisible = dragStartPointInOutput.Transform(RenderBounds, Viewport.Visible);
				Rect output = viewport.OutputWithMargin;

				if (output.Contains(dragStartPointInOutput)) {
					dragLock = DragLock.None;
				}
				else if (output.Left <= dragStartPointInOutput.X && dragStartPointInOutput.X <= output.Right) {
					dragLock = DragLock.OnlyX;
				}
				else {
					dragLock = DragLock.OnlyY;
				}

				isDragging = true;
				CaptureMouse();
			}
			// zooming
			else if (e.ChangedButton == MouseButton.Right && !isDragging) {
				switch (zoomRatioKeepStyle) {
					case KeepZoomRatio.Never:
						shouldKeepRatioWhileZooming = false;
						break;
					case KeepZoomRatio.Always:
						shouldKeepRatioWhileZooming = true;
						break;
					case KeepZoomRatio.ShiftPressed:
						shouldKeepRatioWhileZooming = IsShiftPressed();
						break;
					default:
						break;
				}

				zoomStartPoint = e.GetPosition(this);
				if (viewport.OutputWithMargin.Contains(zoomStartPoint)) {
					isZooming = true;
					AddSelectionAdorner();
					CaptureMouse();
				}
			}
		}

		private static bool IsShiftPressed() {
			return Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
		}

		private KeepZoomRatio zoomRatioKeepStyle = KeepZoomRatio.ShiftPressed;
		public KeepZoomRatio ZoomRatioKeepStyle {
			get { return zoomRatioKeepStyle; }
			set { zoomRatioKeepStyle = value; }
		}

		Rect? zoomRect = null;
		double wheelZoomSpeed = 1.2;
		protected override void OnPreviewMouseWheel(MouseWheelEventArgs e) {
			Point zoomTo = e.GetPosition(this).Transform(viewport.OutputWithMargin, viewport.Visible);

			double delta = -e.Delta;
			double zoomSpeed = Math.Abs(delta / 120);
			zoomSpeed *= wheelZoomSpeed;
			if (delta < 0) {
				zoomSpeed = 1 / zoomSpeed;
			}
			viewport.Visible = viewport.Visible.Zoom(zoomTo, zoomSpeed);
		}

		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			base.OnPreviewMouseMove(e);
			// dragging
			if (isDragging && e.LeftButton == MouseButtonState.Pressed) {
				Point endPoint = CoordinateUtils.Transform(e.GetPosition(this), RenderBounds, Viewport.Visible);

				Point loc = Viewport.Visible.Location;
				Vector shift = dragStartPointInVisible - endPoint;
				if (dragLock == DragLock.OnlyX) {
					shift.Y = 0;
				}
				else if (dragLock == DragLock.OnlyY) {
					shift.X = 0;
				}
				loc += shift;

				Rect visible = Viewport.Visible;
				visible.Location = loc;
				viewport.Visible = visible;
			}
			// zooming
			else if (isZooming && e.RightButton == MouseButtonState.Pressed) {
				Point zoomEndPoint = e.GetPosition(this);
				UpdateZoomRect(zoomEndPoint);
			}
		}

		private void UpdateZoomRect(Point zoomEndPoint) {
			Rect output = viewport.OutputWithMargin;
			Rect tmpZoomRect = new Rect(zoomStartPoint, zoomEndPoint);
			tmpZoomRect = Rect.Intersect(tmpZoomRect, output);

			shouldKeepRatioWhileZooming = zoomRatioKeepStyle == KeepZoomRatio.Always || (zoomRatioKeepStyle == KeepZoomRatio.ShiftPressed && IsShiftPressed());
			if (shouldKeepRatioWhileZooming) {
				double currZoomRatio = tmpZoomRect.Width / tmpZoomRect.Height;
				double zoomRatio = output.Width / output.Height;
				if (currZoomRatio < zoomRatio) {
					double oldHeight = tmpZoomRect.Height;
					double height = tmpZoomRect.Width / zoomRatio;
					tmpZoomRect.Height = height;
					if (!tmpZoomRect.Contains(zoomStartPoint)) {
						tmpZoomRect.Offset(0, oldHeight - height);
					}
				}
				else {
					double oldWidth = tmpZoomRect.Width;
					double width = tmpZoomRect.Height * zoomRatio;
					tmpZoomRect.Width = width;
					if (!tmpZoomRect.Contains(zoomStartPoint)) {
						tmpZoomRect.Offset(oldWidth - width, 0);
					}
				}
			}

			zoomRect = tmpZoomRect;
			UpdateSelectionAdorner();
		}

		protected override void OnPreviewMouseUp(MouseButtonEventArgs e) {
			base.OnPreviewMouseUp(e);
			if (e.ChangedButton == MouseButton.Left) {
				isDragging = false;
				ReleaseMouseCapture();
				Point endPoint = CoordinateUtils.Transform(e.GetPosition(this), RenderBounds, Viewport.Visible);

				// focusing on LMB click
				if (endPoint == dragStartPointInVisible) {
					Keyboard.Focus(this);
				}
			}
			else if (e.ChangedButton == MouseButton.Right && isZooming) {
				isZooming = false;
				if (zoomRect.HasValue) {
					Rect output = viewport.OutputWithMargin;

					Point p1 = zoomRect.Value.TopLeft.Transform(output, viewport.Visible);
					Point p2 = zoomRect.Value.BottomRight.Transform(output, viewport.Visible);
					viewport.Visible = new Rect(p1, p2);

					zoomRect = null;
					ReleaseMouseCapture();
					RemoveSelectionAdorner();
				}
				else {
					// focusing on RMB click
					Keyboard.Focus(this);
				}
			}
		}

		private static bool IsShift(Key key) {
			return key == Key.LeftShift || key == Key.RightShift;
		}

		// todo проверить работу Шифта
		protected override void OnKeyDown(KeyEventArgs e) {
			if (IsShift(e.Key) && isZooming && zoomRatioKeepStyle == KeepZoomRatio.ShiftPressed) {
				Debug.WriteLine("Shift down");
				UpdateSelectionAdorner();
			}

			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e) {
			if (IsShift(e.Key) && isZooming && zoomRatioKeepStyle == KeepZoomRatio.ShiftPressed) {
				Debug.WriteLine("Shift up");
				UpdateSelectionAdorner();
			}

			base.OnKeyUp(e);
		}

		protected override void OnLostFocus(RoutedEventArgs e) {
			if (isZooming) {
				RemoveSelectionAdorner();
			}

			base.OnLostFocus(e);
		}

		private double zoomCoeff = 0.8;
		protected override void OnTextInput(TextCompositionEventArgs e) {
			double zoom = 0;
			switch (e.Text) {
				case "=":
				case "+":
					zoom = zoomCoeff;
					break;
				case "-":
					zoom = 1 / zoomCoeff;
					break;
				default:
					break;
			}

			if (zoom != 0) {
				Point mousePos = Mouse.GetPosition(this);
				Rect output = viewport.OutputWithMargin;

				if (output.Contains(mousePos)) {
					viewport.Visible = Zoom(zoom, mousePos.Transform(output, viewport.Visible));
				}
				else if (IsRectContainsPointX(output, mousePos)) {
					viewport.Visible = ZoomX(zoom);
				}
				else if (IsRectContainsPointY(output, mousePos)) {
					viewport.Visible = ZoomY(zoom);
				}
			}

			base.OnTextInput(e);
		}

		#endregion

		bool adornerAdded = false;
		RectangleSelectionAdorner selectionAdorner;
		AdornerLayer adornerLayer;
		private void AddSelectionAdorner() {
			if (!adornerAdded) {
				selectionAdorner = new RectangleSelectionAdorner(this);
				selectionAdorner.Border = zoomRect;
				adornerLayer.Add(selectionAdorner);
			}
			adornerAdded = true;
		}

		private void RemoveSelectionAdorner() {
			adornerLayer.Remove(selectionAdorner);
			Debug.Assert(adornerAdded);
			adornerAdded = false;
		}

		private void UpdateSelectionAdorner() {
			selectionAdorner.Border = zoomRect;
			selectionAdorner.InvalidateVisual();
		}

		private Rect Zoom(double zoomCoeff) {
			return viewport.Visible.Zoom(viewport.Visible.GetCenter(), zoomCoeff);
		}

		private Rect Zoom(double zoomCoeff, Point point) {
			return viewport.Visible.Zoom(point, zoomCoeff);
		}

		private Rect ZoomX(double zoomCoeff) {
			return viewport.Visible.ZoomX(viewport.Visible.GetCenter(), zoomCoeff);
		}

		private Rect ZoomY(double zoomCoeff) {
			return viewport.Visible.ZoomY(viewport.Visible.GetCenter(), zoomCoeff);
		}

		private static bool IsRectContainsPointX(Rect rect, Point p) {
			return rect.X <= p.X && p.X <= rect.TopRight.X;
		}

		private static bool IsRectContainsPointY(Rect rect, Point p) {
			return rect.Y <= p.Y && p.Y <= rect.BottomRight.Y;
		}

		public LayersCollection Layers {
			get { return LayersCollection.LoadFrom(Children); }
		}

		private enum DragLock {
			OnlyX,
			OnlyY,
			None
		}

		#region INotifyCollectionChanged Members

		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}

	public enum KeepZoomRatio {
		Never,
		Always,
		ShiftPressed
	}
}
