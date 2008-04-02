using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

namespace ScientificStudio.Charting {
	public delegate void ViewportRectChangedHandler(Viewport2D viewport, DependencyPropertyChangedEventArgs e);

	public class Viewport2D : FrameworkContentElement {
		public Viewport2D() {
			sharedViewports.CollectionChanged += attachedViewports_CollectionChanged;
		}

		#region EntireDomain property
		public Rect EntireDomain {
			get { return (Rect)GetValue(EntireDomainProperty); }
			set { SetValue(EntireDomainProperty, value); }
		}

		public static readonly DependencyProperty EntireDomainProperty =
			DependencyProperty.Register(
				"EntireDomain",
				typeof(Rect),
				typeof(Viewport2D),
				new PropertyMetadata(new Rect(0, 0, 1, 1)));
		#endregion

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

		// todo Notify about changes
		private Thickness margin = new Thickness(30, 0, 0, 30);
		public Thickness Margin {
			get { return margin; }
			set { margin = value; }
		}

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
