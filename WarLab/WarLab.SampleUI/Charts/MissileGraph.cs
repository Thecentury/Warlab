﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScientificStudio.Charting;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows;


namespace WarLab.SampleUI.Charts {
	public sealed class MissileGraph : WarGraph {
		public ISpriteSource SpriteSource { get; set; }
		const double missileLength = 10; // px in each direction

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Point center = SpriteSource.Position.Projection2D;
			center = center.Transform(state.Visible, state.OutputWithMargin);

			Vector2D orientation = SpriteSource.Orientation.Projection2D;
			Point start = new Point(
				center.X + missileLength * orientation.X,
				center.Y - missileLength * orientation.Y);

			Point end = new Point(
				center.X - missileLength * orientation.X,
				center.Y + missileLength * orientation.Y);

			Pen linePen = new Pen(Brushes.Crimson, 3.5);
			Pen outlinePen = new Pen(Brushes.Black, 4);
			dc.DrawLine(outlinePen, start, end);
			dc.DrawLine(linePen, start, end);
		}
	}
}
