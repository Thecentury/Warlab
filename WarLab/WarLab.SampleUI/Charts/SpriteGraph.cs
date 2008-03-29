using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows;
using ScientificStudio.Charting;

namespace WarLab.SampleUI.Charts {
	public class SpriteGraph : GraphicalObject {
		public ISpriteSource SpriteSource {
			get { return (ISpriteSource)GetValue(SpriteSourceProperty); }
			set { SetValue(SpriteSourceProperty, value); }
		}

		public static readonly DependencyProperty SpriteSourceProperty =
			DependencyProperty.Register(
			  "SpriteSource",
			  typeof(ISpriteSource),
			  typeof(SpriteGraph),
			  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public ImageSource SpriteImage {
			get { return (ImageSource)GetValue(SpriteImageProperty); }
			set { SetValue(SpriteImageProperty, value); }
		}

		public static readonly DependencyProperty SpriteImageProperty =
			DependencyProperty.Register(
			  "SpriteImage",
			  typeof(ImageSource),
			  typeof(SpriteGraph),
			  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public Size SpriteSize {
			get { return (Size)GetValue(SpriteSizeProperty); }
			set { SetValue(SpriteSizeProperty, value); }
		}

		public static readonly DependencyProperty SpriteSizeProperty =
			DependencyProperty.Register(
			  "SpriteSize",
			  typeof(Size),
			  typeof(SpriteGraph),
			  new FrameworkPropertyMetadata(new Size(20, 20)));

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			if (SpriteSource == null) return;

			Vector2D orientation = SpriteSource.Orientation;
			double angle = Math.Atan2(orientation.Y, orientation.X);
			angle = MathHelper.AngleToDegrees(angle);

			Vector2D pos2D = SpriteSource.Position.Projection2D;
			Point transformedPos = CoordinateUtils.Transform(new Point(pos2D.X, pos2D.Y), state.Visible, state.OutputWithMargin);

			dc.PushTransform(new RotateTransform(angle, pos2D.X, pos2D.Y));
			dc.DrawImage(SpriteImage, MathHelper.CreateRectFromCenterSize(transformedPos, SpriteSize));
			dc.Pop();
		}
	}
}
