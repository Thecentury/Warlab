using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.Isoline;
using System.Windows.Media;
using System.Windows;
using System.Globalization;

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
