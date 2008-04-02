using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using ScientificStudio.Charting.GraphicalObjects;

namespace ScientificStudio.Charting {
	public delegate void ViewportRectChangedHandler(Viewport2D viewport, DependencyPropertyChangedEventArgs e);

	public class Viewport2D : FrameworkContentElement {
		public Viewport2D() {
			sharedViewports.CollectionChanged += attachedViewports_CollectionChanged;
		}

		#region RectChanged event
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			RaiseRectChangedEvent(e);
		}

		internal event ViewportRectChangedHandler RectChanged;
		private void RaiseRectChangedEvent(DependencyPropertyChangedEventArgs e) {
			ViewportRectChangedHandler handler = RectChanged;
			if (handler != null) {
				handler(this, e);
			}
		}
		#endregion

		#region Output property
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rect Output {
			get { return (Rect)GetValue(OutputProperty); }
			set { SetValue(OutputProperty, value); }
		}

		public static readonly DependencyProperty OutputProperty =
			DependencyProperty.Register("Output", typeof(Rect), typeof(Viewport2D));
		#endregion

		internal void AttachToPlotter(ChartPlotter plotter) {
			plotter.CollectionChanged += ParentPlotter_CollectionChanged;
		}

		internal void DetachFromPlotter(ChartPlotter plotter) {
			plotter.CollectionChanged -= ParentPlotter_CollectionChanged;
		}

		public void FitToView() {
			ClearValue(VisibleProperty);
			CoerceValue(VisibleProperty);
		}

		private void ParentPlotter_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					GraphicalObject graph = e.NewItems[0] as GraphicalObject;
					if (graph != null) {
						graph.ContentBoundsChanged += graph_ContentBoundsChanged;
					}
					break;
				case NotifyCollectionChangedAction.Move:
					break;
				case NotifyCollectionChangedAction.Remove:
					graph = e.OldItems[0] as GraphicalObject;
					if (graph != null) {
						graph.ContentBoundsChanged -= graph_ContentBoundsChanged;
					}
					break;
				case NotifyCollectionChangedAction.Replace:
					break;
				case NotifyCollectionChangedAction.Reset:
					break;
				default:
					break;
			}
			UpdateVisible();
		}

		private void graph_ContentBoundsChanged(object sender, EventArgs e) {
			UpdateVisible();
		}

		private void UpdateVisible() {
			if (ClipToBounds && ReadLocalValue(VisibleProperty) == DependencyProperty.UnsetValue) {
				CoerceValue(VisibleProperty);
			}
		}

		private ChartPlotter ParentPlotter {
			get { return Parent as ChartPlotter; }
		}

		#region Visible property

		public Rect Visible {
			get { return (Rect)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}

		public static readonly DependencyProperty VisibleProperty =
			DependencyProperty.Register("Visible", typeof(Rect), typeof(Viewport2D),
			new FrameworkPropertyMetadata(
				new Rect(0, 0, 1, 1),
				OnVisibleChanged,
				OnCoerceVisible));

		private Rect CoerceVisible(Rect rect) {
			foreach (SharedViewport v in sharedViewports) {
				if (v.ShareType == ShareType.Get && v.Viewport != null) {
					rect = RectShareHelper.Combine(rect, v.Viewport.Visible, v.VisibleShare);
				}
			}

			bool isDefaultValue = rect == (Rect)VisibleProperty.DefaultMetadata.DefaultValue;
			if (isDefaultValue && ClipToBounds && ReadLocalValue(VisibleProperty) == DependencyProperty.UnsetValue) {
				Rect bounds = Rect.Empty;
				foreach (var g in ParentPlotter.GraphChildren) {
					var graph = g as GraphicalObject;
					if (graph != null && graph.Visibility == Visibility.Visible) {
						bounds.Union(graph.ContentBounds);
					}
				}
				if (!bounds.IsEmpty) {
					bounds = CoordinateUtils.RectZoom(bounds, CoordinateUtils.RectCenter(bounds), clipToBoundsFactor);
				}
				else {
					bounds = new Rect(0, 0, 1, 1);
				}
				rect.Union(bounds);
				if (rect.IsEmpty || rect.Width == 0 || rect.Height == 0) {
					rect = (Rect)VisibleProperty.DefaultMetadata.DefaultValue;
				}
			}

			return rect;
		}

		private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Viewport2D v = (Viewport2D)d;
			v.OnVisibleChanged();
		}

		private void OnVisibleChanged() {
			foreach (SharedViewport v in sharedViewports) {
				if (v.ShareType == ShareType.Equal && v.Viewport != null) {
					v.Viewport.Visible = RectShareHelper.Combine(v.Viewport.Visible, Visible, v.VisibleShare);
				}
			}
		}

		private static object OnCoerceVisible(DependencyObject d, object newValue) {
			Viewport2D viewport = (Viewport2D)d;
			Rect newRect = viewport.CoerceVisible((Rect)newValue);

			if (newRect.Width == 0 || newRect.Height == 0) {
				// doesn't apply rects with zero square
				return DependencyProperty.UnsetValue;
			}
			else {
				return newRect;
			}
		}

		#endregion

		#region Attached viewports

		private void attachedViewports_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null) {
				foreach (object item in e.NewItems) {
					AddLogicalChild(item);
				}
			}
			if (e.OldItems != null) {
				foreach (object item in e.OldItems) {
					RemoveLogicalChild(item);
				}
			}
			CoerceValue(VisibleProperty);
		}

		private ObservableCollection<SharedViewport> sharedViewports = new ObservableCollection<SharedViewport>();
		public ObservableCollection<SharedViewport> SharedViewports {
			get { return sharedViewports; }
		}

		#endregion

		private double clipToBoundsFactor = 1.10;
		/// <summary>
		/// Gets or sets the clip to bounds factor.
		/// </summary>
		/// <value>The clip to bounds factor.</value>
		public double ClipToBoundsFactor {
			get { return clipToBoundsFactor; }
			set {
				if (clipToBoundsFactor != value) {
					clipToBoundsFactor = value;
					UpdateVisible();
				}
			}
		}

		private bool clipToBounds = true;
		/// <summary>
		/// Gets or sets a value indicating whether viewport clips 
		/// in its initial visible rect to bounds of graphs.
		/// </summary>
		[DefaultValue(true)]
		public bool ClipToBounds {
			get { return clipToBounds; }
			set {
				if (clipToBounds != value) {
					clipToBounds = value;
					if (value) {
						UpdateVisible();
					}
				}
			}
		}

		// todo Notify about changes
		private Thickness margin = new Thickness(30, 0, 0, 30);
		/// <summary>
		/// Gets or sets the margin.
		/// </summary>
		/// <value>The margin.</value>
		public Thickness Margin {
			get { return margin; }
			set { margin = value; }
		}

		/// <summary>
		/// Gets the output with margin.
		/// </summary>
		/// <value>The output with margin.</value>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Rect OutputWithMargin {
			get {
				Rect output = Output;
				output.Offset(margin.Left, margin.Top);
				output.Width = Math.Max(output.Width - margin.Left - margin.Right, 0);
				output.Height = Math.Max(output.Height - margin.Top - margin.Bottom, 0);
				return output;
			}
		}
	}
}
