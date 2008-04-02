using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting {
	[Flags]
	public enum ViewportRectShare {
		None = 0x0,
		X = 0x1,
		Width = 0x2,
		Horizontal = X | Width,
		Y = 0x4,
		Height = 0x8,
		Vertical = Y | Height,
		All = Horizontal | Vertical
	}


	public enum ShareType {
		Get,
		Equal
	}

	public sealed class SharedViewport : FrameworkContentElement {
		private Viewport2D ParentViewport {
			get { return (Viewport2D)Parent; }
		}

		private ViewportRectShare visibleShare = ViewportRectShare.All;
		public ViewportRectShare VisibleShare {
			get { return visibleShare; }
			set { visibleShare = value; }
		}

		private ShareType shareType = ShareType.Get;
		public ShareType ShareType {
			get { return shareType; }
			set { shareType = value; }
		}

		public Viewport2D Viewport {
			get { return (Viewport2D)GetValue(ViewportProperty); }
			set { SetValue(ViewportProperty, value); }
		}

		private void AttachViewport(Viewport2D viewport) {
			viewport.RectChanged += viewport_RectChanged;
		}

		private void DetachViewport(Viewport2D viewport) {
			viewport.RectChanged -= viewport_RectChanged;
		}

		private void viewport_RectChanged(Viewport2D viewport, DependencyPropertyChangedEventArgs e) {
			UpdateParentVisible();
		}

		private void UpdateParentVisible() {
			ParentViewport.Visible = RectShareHelper.Combine(
				ParentViewport.Visible,
				Viewport.Visible,
				VisibleShare);
		}

		public static readonly DependencyProperty ViewportProperty =
			DependencyProperty.Register(
			  "Viewport",
			  typeof(Viewport2D),
			  typeof(SharedViewport),
			  new FrameworkPropertyMetadata(null, OnViewportChanged));

		private static void OnViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SharedViewport v = (SharedViewport)d;
			if (e.NewValue != null) {
				v.AttachViewport(e.NewValue as Viewport2D);
			}
			if (e.OldValue != null) {
				v.DetachViewport(e.OldValue as Viewport2D);
			}
			v.UpdateParentVisible();
		}
	}
}
