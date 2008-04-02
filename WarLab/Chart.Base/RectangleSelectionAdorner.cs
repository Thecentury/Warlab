using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Media;

namespace ScientificStudio.Charting {
	internal sealed class RectangleSelectionAdorner : Adorner {
		internal Rect? Border = null;

		internal Brush Fill = new SolidColorBrush(Color.FromArgb(60, 100, 100, 100));
		internal Pen Pen;

		public RectangleSelectionAdorner(UIElement element)
			: base(element) {
			Pen = new Pen(Brushes.Black, 1.0);
		}

		protected override void OnRender(DrawingContext dc) {
			if (Border.HasValue) {
				dc.DrawRectangle(Fill, Pen, Border.Value);
			}
		}
	}
}
