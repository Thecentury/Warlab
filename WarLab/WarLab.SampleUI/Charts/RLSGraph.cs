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
using WarLab.WarObjects;
using WarLab.AI;

namespace WarLab.SampleUI.Charts {
	public class RLSGraph : WarGraph {
		public RLSGraph() {
			// todo нужно ли это?
			RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.LowQuality);
			IsHitTestVisible = false;
		}

		public StaticObject StaticObject {
			get { return (StaticObject)GetValue(SpriteSourceProperty); }
			set { SetValue(SpriteSourceProperty, value); }
		}

		public static readonly DependencyProperty SpriteSourceProperty =
			DependencyProperty.Register(
			  "SpriteSource",
			  typeof(StaticObject),
			  typeof(RLSGraph),
			  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public ImageSource SpriteImage {
			get { return (ImageSource)GetValue(SpriteImageProperty); }
			set { SetValue(SpriteImageProperty, value); }
		}

		public static readonly DependencyProperty SpriteImageProperty =
			DependencyProperty.Register(
			  "SpriteImage",
			  typeof(ImageSource),
			  typeof(RLSGraph),
			  new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));

		public Size SpriteSize {
			get { return (Size)GetValue(SpriteSizeProperty); }
			set { SetValue(SpriteSizeProperty, value); }
		}

		public static readonly DependencyProperty SpriteSizeProperty =
			DependencyProperty.Register(
			  "SpriteSize",
			  typeof(Size),
			  typeof(RLSGraph),
			  new FrameworkPropertyMetadata(new Size(20, 20)));

		public void DoUpdate() {
			MakeDirty();
			InvalidateVisual();
		}

		private RLS Rls {
			get { return StaticObject as RLS; }
		}

		private RLSAI Ai {
			get { return Rls.AI as RLSAI; }
		}

		readonly Brush brush = Brushes.Red;
		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			if (StaticObject == null) return;

			Vector2D pos2D = StaticObject.Position.Projection2D;

			Point transformedPos = CoordinateUtils.Transform(new Point(pos2D.X, pos2D.Y), state.Visible, state.OutputWithMargin);

			Size size = new Size(SpriteImage.Width, SpriteImage.Height);

			dc.DrawImage(SpriteImage, MathHelper.CreateRectFromCenterSize(transformedPos, size));
			double radius = Rls.CoverageRadius;
			double radiusX = radius / state.Visible.Width * state.OutputWithMargin.Width;
			double radiusY = radius / state.Visible.Height * state.OutputWithMargin.Height;

			foreach (var t in Ai.AllTrajectories) {
				Point pos = CoordinateUtils.Transform(t.Position.Projection2D, state.Visible, state.OutputWithMargin);
				dc.DrawEllipse(brush, null, pos, 3, 3);

				if (t.HasDirection) {
					Vector2D orient = t.Direction.Projection2D;
					const double length = 20;
					Point p2 = new Point(pos.X + orient.X * length,
						pos.Y - orient.Y * length);

					Pen pen = new Pen(brush, 2);

					dc.DrawLine(pen, pos, p2);
				}
			}

			dc.DrawEllipse(null, new Pen(Brushes.Green, 2), transformedPos, radiusX, radiusY);
		}
	}
}
