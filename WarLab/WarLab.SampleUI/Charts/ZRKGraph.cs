using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScientificStudio.Charting.GraphicalObjects;
using WarLab.WarObjects;
using System.Windows;
using ScientificStudio.Charting;

namespace WarLab.SampleUI.Charts {
	public sealed class ZRKGraph : StaticObjectGraph {
		private ZRKBase RenderedZRK {
			get { return StaticObject as ZRKBase; }
		}

		const double topMargin = 5; // px
		const double channelWidth = 70; //px
		const double channelHeight = 7; // px
		const double channelMargin = 5; // px

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			if (StaticObject == null) return;
			if (SpriteImage == null) return;

			Color coverageFillColor = Colors.Red;
			coverageFillColor.A = 40;
			Brush coverageFill = new SolidColorBrush(coverageFillColor);
			
			coverageFillColor.A = 100;
			Brush coverageOutlineBrush = new SolidColorBrush(coverageFillColor);
			Pen coverageOutline = new Pen(coverageOutlineBrush, 2);

			Vector2D pos2D = StaticObject.Position.Projection2D;

			Point spriteCenter = GetSpriteCenter(state);

			double radius = RenderedZRK.CoverageRadius;
			double radiusX = radius / state.Visible.Width * state.OutputWithMargin.Width;
			double radiusY = radius / state.Visible.Height * state.OutputWithMargin.Height;
			dc.DrawEllipse(coverageFill, coverageOutline, spriteCenter, radiusX, radiusY);

			base.OnRenderCore(dc, state);

			Size spriteSize = SpriteSize;

			Rect channelRect = MathHelper.CreateRectFromCenterSize(spriteCenter, channelWidth, channelHeight);
			channelRect.Y -= topMargin + spriteSize.Height / 2;

			Pen channelBoundsPen = new Pen(Brushes.Black, 1.5);
			Brush interiorFill = Brushes.GreenYellow;
			var channels = RenderedZRK.Channels;
			
			for (int i = 0; i < channels.Length; i++) {
				Rect bounds = channelRect;
				bounds.Y -= (channelHeight + channelMargin) * i;

				Rect loadedBounds = bounds;
				loadedBounds.Width *= 1 - channels[i].TimeToReload.TotalSeconds / ZRK.ChannelReloadTime.TotalSeconds;
				dc.DrawRectangle(interiorFill, null, loadedBounds);

				dc.DrawRectangle(null, channelBoundsPen, bounds);
			}
		}
	}
}
