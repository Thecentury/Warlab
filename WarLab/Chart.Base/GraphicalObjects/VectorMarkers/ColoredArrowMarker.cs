using System;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.Auxilliary;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects.VectorMarkers {
	public sealed class ColoredArrowMarker : VectorMarker {

		#region Properties

		private double arrowAngle = 0.2;
		public double ArrowAngle {
			get { return arrowAngle; }
			set { arrowAngle = value; }
		}

		private double arrowLength = 0.6;
		public double ArrowLength {
			get { return arrowLength; }
			set { arrowLength = value; }
		}

		private bool normalizeLength = true;
		public bool NormalizeLength {
			get { return normalizeLength; }
			set { normalizeLength = value; }
		}

		#endregion

		private IPalette palette = new HSBPalette();
		public IPalette Palette {
			get { return palette; }
			set { palette = value; }
		}

		public override void Render(DrawingContext dc, Point pos, Vector2D dir) {
#if DEBUG
			base.Render(dc, pos, dir);
#endif
			double len = dir.Length;

			if (normalizeLength)
				dir.Normalize();

			double ratio = (len - min) / (max - min);
			MathHelper.Clamp_01(ref ratio);
			Color c = palette.GetColor(ratio);

			Brush brush = new SolidColorBrush(c);
			Pen pen = new Pen(brush, 2);

			// start point
			dc.DrawEllipse(brush, pen, pos, 2, 2);

			// do not display other parts, if vector is too short
			if (dir.LengthSquared <= 0.0001) return;
			
			// body of the vector
			Point end = new Point(
				pos.X + dir.x * MarkerSize.Width,
				pos.Y + dir.y * MarkerSize.Height);
			dc.DrawLine(pen, pos, end);
			double angle = Math.Atan2(dir.y, dir.x);

			// arrow - 2 lines
			double absArrowLenX = MarkerSize.Width * arrowLength;
			double absArrowLenY = MarkerSize.Height * arrowLength;
			dc.DrawLine(pen, end, new Point(
				end.X - absArrowLenX * Math.Cos(angle + arrowAngle),
				end.Y - absArrowLenY * Math.Sin(angle + arrowAngle)));
			dc.DrawLine(pen, end, new Point(
				end.X - absArrowLenX * Math.Cos(angle - arrowAngle),
				end.Y - absArrowLenY * Math.Sin(angle - arrowAngle)));
		}

		private double min;
		private double max;
		public override void Init(VectorField2d grid) {
			base.Init(grid);

			IVectorArray2d array = grid.Data;
			int width = array.Width;
			int height = array.Height;

			min = array[0, 0].Length;
			max = min;

			for (int ix = 0; ix < width; ix++) {
				for (int iy = 0; iy < height; iy++) {
					Vector2D vec = array[ix, iy];

					double len = vec.Length;
					if (len < min) min = len;
					if (len > max) max = len;
				}
			}
		}
	}
}
