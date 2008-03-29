using System;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.Auxilliary;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects.VectorMarkers {
	public sealed class ColoredTriangleMarker : VectorMarker {

		private IPalette palette = new HSBPalette();
		public IPalette Palette {
			get { return palette; }
			set { palette = value; }
		}

		private double min;
		private double max;
		public override void Init(VectorField2d field) {
			base.Init(field);

			IVectorArray2d array = field.Data;
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

		private const double PI_div_2 = Math.PI / 2;

		private double endX;
		private double endY;
		private double midX;
		private double midY;
		double baseSizeX;
		double baseSizeY;
		public override void PreRenderInit(Size markerSize) {
			base.PreRenderInit(markerSize);
			
			double centerShift = 0.3;
			endX = markerSize.Width * (1 - centerShift);
			endY = markerSize.Height * (1 - centerShift);

			midX = markerSize.Width * centerShift;
			midY = markerSize.Height * centerShift;

			baseSizeX = 0.2 * markerSize.Width;
			baseSizeY = 0.2 * markerSize.Height;
		}

		public override void Render(DrawingContext dc, Point pos, Vector2D dir) {
#if DEBUG
			base.Render(dc, pos, dir);
#endif
			double len = dir.Length;

			dir.Normalize();

			double ratio = (len - min) / (max - min);
			MathHelper.Clamp_01(ref ratio);
			Color c = palette.GetColor(ratio);

			Brush brush = new SolidColorBrush(c);
			Pen pen = new Pen(brush, 1);

			// body of the vector
			Point end = new Point(
				pos.X + dir.x * endX,
				pos.Y + dir.y * endY);
			Point baseMid = new Point(
				pos.X - dir.x * midX,
				pos.Y - dir.y * midY);
			double angle = Math.Atan2(dir.y, dir.x);

			double angleLeft = angle + PI_div_2;
			double angleRight = angle - PI_div_2;


			Point left = new Point(
				baseMid.X + Math.Cos(angleLeft) * baseSizeX,
				baseMid.Y + Math.Sin(angleLeft) * baseSizeY);

			Point right = new Point(
				baseMid.X + Math.Cos(angleRight) * baseSizeX,
				baseMid.Y + Math.Sin(angleRight) * baseSizeY);

			StreamGeometry g = new StreamGeometry();
			using (StreamGeometryContext ctx = g.Open()) {
				ctx.BeginFigure(end, true, true);
				ctx.LineTo(right, true, true);
				ctx.LineTo(left, true, true);
			}
			g.Freeze();

			dc.DrawGeometry(brush, pen, g);
		}
	}
}
