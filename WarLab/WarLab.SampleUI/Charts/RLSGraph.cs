#define full

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
		public StaticObject StaticObject {
			get { return (StaticObject)GetValue(SpriteSourceProperty); }
			set {
				SetValue(SpriteSourceProperty, value);
				WarObject = value;
			}
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

			RLSAI ai = Rls.AI as RLSAI;
			double turnRatio = ai.FromPrevTurn.TotalSeconds / Rls.RotationPeriod.TotalSeconds;
			double angle = 360 * turnRatio;

			dc.PushTransform(new RotateTransform(angle, transformedPos.X, transformedPos.Y));

			dc.DrawImage(SpriteImage, MathHelper.CreateRectFromCenterSize(transformedPos, size));

			dc.Pop();

			double radius = Rls.CoverageRadius;
			double radiusX = radius / state.Visible.Width * state.OutputWithMargin.Width;
			double radiusY = radius / state.Visible.Height * state.OutputWithMargin.Height;

			foreach (var t in Ai.AllTrajectories) {
#if false
				Point pos = CoordinateUtils.Transform(t.Position.Projection2D, state.Visible, state.OutputWithMargin);
#else
				Point pos = CoordinateUtils.Transform(t.ExtrapolatedPosition(World.Instance.Time.TotalTime).Projection2D, state.Visible, state.OutputWithMargin);
#endif
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

			Color fillColor = Colors.Green;
			fillColor.A = 60;
			Brush rlsFill = new SolidColorBrush(fillColor);
			dc.DrawEllipse(rlsFill, new Pen(Brushes.Green, 2), transformedPos, radiusX, radiusY);

			Color lineColor = Colors.Green;
			lineColor.A = 160;

#if !full
			Point radarLineEnd = new Point(transformedPos.X + radiusX * Math.Cos(Rls.RadarAngle), transformedPos.Y - radiusY * Math.Sin(Rls.RadarAngle));
			dc.DrawLine(new Pen(new SolidColorBrush(lineColor), 2), transformedPos, radarLineEnd);
#endif
		}
	}
}
