using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects.VectorMarkers {
	public sealed class TextMarker : VectorMarker {
		
		public override void Render(DrawingContext dc, Point pos, Vector2D dir) {
#if DEBUG
			base.Render(dc, pos, dir);
#endif

			FormattedText text = new FormattedText(dir.ToString(), CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
				new Typeface("Consolas"), 12, Brushes.Black);
			dc.DrawText(text, pos);
		}

	}
}
