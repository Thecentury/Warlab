using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace ScientificStudio.Charting.Isoline {
	public sealed class LevelLine {
		private Color color = Colors.LightBlue;
		public Color Color {
			get { return color; }
			set { color = value; }
		}

		private Point startPoint;
		public Point StartPoint {
			get { return startPoint; }
			set { startPoint = value; }
		}

		private readonly List<Point> otherPoints = new List<Point>();
		public List<Point> OtherPoints {
			get { return otherPoints; }
		}
	}

	public sealed class TextLabel {
		private Vector rotation = new Vector(0, 1);
		public Vector Rotation {
			get { return rotation; }
			set { rotation = value; }
		}

		private string text = "";
		public string Text {
			get { return text; }
			set { text = value; }
		}

		private Point position = new Point();
		public Point Position {
			get { return position; }
			set { position = value; }
		}
	}

	public sealed class IsolineCollection {
		private readonly List<TextLabel> labels = new List<TextLabel>();
		public List<TextLabel> Labels {
			get { return labels; }
		}

		public void AddTextLabel(TextLabel label) {
			labels.Add(label);
		}

		private readonly List<LevelLine> lines = new List<LevelLine>();
		public List<LevelLine> Lines {
			get { return lines; }
		}

		public void StartLine(Point p, Color color) {
			LevelLine segment = new LevelLine { StartPoint = p, Color = color };
			lines.Add(segment);
		}

		public void AddPoint(Point p) {
			lines[lines.Count - 1].OtherPoints.Add(p);
		}

		public void Clear() {
			lines.Clear();
			labels.Clear();
		}
	}
}
