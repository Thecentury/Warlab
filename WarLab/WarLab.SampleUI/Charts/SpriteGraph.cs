using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows;
using ScientificStudio.Charting;
using System.Diagnostics;
using WarLab.SampleUI.WarObjects;

namespace WarLab.SampleUI.Charts {
	public class SpriteGraph : WarGraph {
		public SpriteGraph() {
			IsHitTestVisible = false;
		}

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

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			if (SpriteSource == null) return;

			Vector2D orientation = SpriteSource.Orientation.Projection2D;
			double angle = Math.Atan2(orientation.Y, orientation.X);
			angle = 90 - MathHelper.AngleToDegrees(angle);

			Vector2D pos2D = SpriteSource.Position.Projection2D;

			Point transformedPos = CoordinateUtils.Transform(new Point(pos2D.X, pos2D.Y), state.Visible, state.OutputWithMargin);

			Size size = new Size(SpriteImage.Width, SpriteImage.Height);
			if (SpriteImage.Width < 20) {
				// для правильного отображения EnemyPlane.png - он почему-то считает, что его размер 16x14
				size = new Size(100, 90);
			}

			dc.PushTransform(new RotateTransform(angle, transformedPos.X, transformedPos.Y));
#if !true
			const double ellipseSize = 5;
			dc.DrawEllipse(Brushes.Red, null, transformedPos, ellipseSize, ellipseSize);
#else
			dc.DrawImage(SpriteImage, MathHelper.CreateRectFromCenterSize(transformedPos, size));
#endif
			dc.Pop();
		}
	}
}
