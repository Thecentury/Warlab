﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.IO;
using System.Windows;
using System.Globalization;
using ScientificStudio.Charting.GraphicalObjects.Filters;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace ScientificStudio.Charting.GraphicalObjects {
	public class Coastline : GraphicalObject, ISupportInitialize {
		private readonly string delimiter = "nan nan";

		#region Filtering quality

		public static readonly DependencyProperty FilteringQualityProperty =
			DependencyProperty.Register(
			"FilteringQuality",
			typeof(double),
			typeof(Coastline),
			new FrameworkPropertyMetadata(
				178.0,
				FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
				OnFilteringQualityChanged),
			OnValidateFilteringQuality);

		public double FilteringQuality {
			get { return (double)GetValueSync(FilteringQualityProperty); }
			set { SetValueAsync(FilteringQualityProperty, value); }
		}

		private static bool OnValidateFilteringQuality(object value) {
			double degrees = (double)value;
			return degrees >= 0 && degrees <= 180;
		}

		private static void OnFilteringQualityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Coastline coast = (Coastline)d;
			coast.Filter();
		}

		#endregion

		string filePath = "";
		public string FilePath {
			get { return filePath; }
			set { filePath = value; }
		}

		public override void EndInit() {
			// we don't load map if we are in design mode or path to file with map is not specified
			if (!DesignerProperties.GetIsInDesignMode(this) && filePath != "") {
				using (StreamReader reader = new StreamReader(filePath)) {
					LoadFromStreamReader(reader);
				}
			}
			base.EndInit();
		}

		private void Init() {
			RenderTarget = RenderTo.Image;
		}

		public Coastline() { Init(); }

		public Coastline(string path) {
			using (StreamReader reader = new StreamReader(path)) {
				LoadFromStreamReader(reader);
			}
			Init();
		}

		public Coastline(Stream stream) {
			using (StreamReader reader = new StreamReader(stream)) {
				LoadFromStreamReader(reader);
			}
			Init();
		}

		List<List<Point>> initialPoints = new List<List<Point>>();
		List<List<Point>> points = new List<List<Point>>();
		InclinationFilter filter = new InclinationFilter();

		private void LoadFromStreamReader(StreamReader reader) {
			// reading and parsing info from stream
			string currStr;
			List<Point> currCurve = new List<Point>();
			while ((currStr = reader.ReadLine()) != null) {
				if (currStr != delimiter) {
					string[] coords = currStr.Split('\t');
					Point p = new Point(Parse(coords[0]), Parse(coords[1]));
					currCurve.Add(p);
				}
				else {
					// current string is delimeter - closing current points seq and adding it
					if (currCurve.Count > 0) {
						points.Add(currCurve);
						currCurve = new List<Point>();
					}
				}
			}

			initialPoints = new List<List<Point>>(points);

			Filter();
		}

		private void Filter() {
			filter.CriticalAngle = FilteringQuality;

			// filtering initial points
			List<List<Point>> filteredPoints = new List<List<Point>>();
			foreach (List<Point> pointList in initialPoints) {
				List<Point> filteredSeq = filter.Filter(pointList);

				List<Point> seqToFilter = null;
				bool addNeeded = true;
				// checking, if previous points seq ends with the same point that this seq begins
				if (filteredPoints.Count > 0) {
					foreach (var seq in filteredPoints) {
						if (seq.Count > 0) {
							Point lastPoint = seq.GetLast();
							if (lastPoint == filteredSeq[0]) {
								// if two seqs can be united, they are.
								seq.AddRange(filteredSeq.Skip(1));
								seqToFilter = seq;
								addNeeded = false;
							}
						}
					}
				}
				// adding new seq, if it was created
				if (filteredSeq.Count > 0 && addNeeded) {
					filteredPoints.Add(filteredSeq);
				}
			}

			// final filtering
			for (int i = 0; i < filteredPoints.Count; i++) {
				filteredPoints[i] = filter.Filter(filteredPoints[i]);
			}

#if DEBUG
			int sum = filteredPoints.Aggregate(0, ((s, list) => s += list.Count));
			SetValue(PointsNumberPropertyKey, sum);
#endif
			points = filteredPoints;
			MakeDirty();
		}

#if DEBUG
		#region PointsNumber
		private static readonly DependencyPropertyKey PointsNumberPropertyKey =
			DependencyProperty.RegisterReadOnly(
			"PointsNumber",
			typeof(int),
			typeof(Coastline),
			new FrameworkPropertyMetadata());

		public static readonly DependencyProperty PointsNumberProperty = PointsNumberPropertyKey.DependencyProperty;

		public int PointsNumber {
			get { return points.Aggregate(0, ((s, list) => s += list.Count)); }
		}
		#endregion
#endif

		private double Parse(string str) {
			return Double.Parse(str, CultureInfo.InvariantCulture);
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect outputWMargin = state.OutputWithMargin;
			Rect output = state.Output;

			if (outputWMargin.Width == 0 || outputWMargin.Height == 0) return;

			StreamGeometry cachedGeom = new StreamGeometry();
			List<List<Point>> transformedPts = new List<List<Point>>();

			foreach (List<Point> list in points) {
				transformedPts.Add(list.Transform(state.Visible, outputWMargin));
			}

			cachedGeom = new StreamGeometry();
			using (StreamGeometryContext context = cachedGeom.Open()) {
				foreach (List<Point> pts in transformedPts) {
					context.BeginFigure(pts[0], false, false);
					context.PolyLineTo(pts, true, true);
				}
			}
			cachedGeom.Freeze();
			dc.DrawGeometry(
				Brushes.LightGray,
				new Pen(Brushes.Black, 1),
				cachedGeom);
		}
	}
}
