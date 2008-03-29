using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.PointSources;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;

namespace ScientificStudio.Charting {
	public sealed class SimplifiedChartPlotter : ChartPlotter {
		/// <summary>
		/// Initializes a new instance of the <see cref="SimplifiedChartPlotter"/> class.
		/// </summary>
		public SimplifiedChartPlotter()
			: base() {
			AddGraph(new Axises());
			AddGraph(new Legend());
			ZoomRatioKeepStyle = KeepZoomRatio.Always;
		}

		/// <summary>
		/// Adds one dimensional graph with random color of line.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		public PointsGraph AddGraph1d(IPointSource pointSource) {
			return AddGraph1d(pointSource, ColorHelper.RandomColorEx());
		}

		/// <summary>
		/// Adds one dimensional graph with specified color of line.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineColor">Color of the line.</param>
		/// <returns></returns>
		public PointsGraph AddGraph1d(IPointSource pointSource, Color lineColor) {
			return AddGraph1d(pointSource, lineColor, 1);
		}

		/// <summary>
		/// Adds one dimensional graph with random color if line.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <returns></returns>
		public PointsGraph AddGraph1d(IPointSource pointSource, double lineThickness) {
			return AddGraph1d(pointSource, ColorHelper.RandomColorEx(), lineThickness);
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineColor">Color of the line.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <returns></returns>
		public PointsGraph AddGraph1d(IPointSource pointSource, Color lineColor, double lineThickness) {
			return AddGraph1d(pointSource, new Pen(new SolidColorBrush(lineColor), lineThickness), null);
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public PointsGraph AddGraph1d(IPointSource pointSource, string description) {
			PointsGraph g = AddGraph1d(pointSource);
			g.Description = new PenDescription(description);
			return g;
		}

		/// <summary>
		/// Adds one dimensional graph.
		/// </summary>
		/// <param name="pointSource">The point source.</param>
		/// <param name="lineThickness">The line thickness.</param>
		/// <param name="description">The description.</param>
		/// <returns></returns>
		public PointsGraph AddGraph1d(IPointSource pointSource, double lineThickness, string description) {
			var g = AddGraph1d(pointSource, lineThickness);
			g.Description = new PenDescription(description);
			return g;
		}

		private PointsGraph AddGraph1d(IPointSource pointSource, Pen linePen, Description description) {
			if (pointSource == null)
				throw new ArgumentNullException("pointSource");
			if (linePen == null)
				throw new ArgumentNullException("linePen");

			PointsGraph g = new PointsGraph
			{
				PointSource = pointSource,
				LinePen = linePen
			};
			if (description != null) {
				g.Description = description;
			}
			AddGraph(g);

			return g;
		}

		protected override void OnRender(DrawingContext dc) {
			base.OnRender(dc);
			// draw border
			Rect border = new Rect(RenderSize);
			dc.DrawRectangle(null, new Pen(Brushes.Black, 1), border);
		}
	}
}
