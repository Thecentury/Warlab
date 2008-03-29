using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using ScientificStudio.Charting.PointSources;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;
using System.Windows;
using ScientificStudio.Charting.GraphicalObjects.Filters;

namespace ScientificStudio.Charting.GraphicalObjects {
	public class DotsGraph : GraphicalObject {
		public DotsGraph() {
			Legend.SetShowInLegend(this, true);
		}

		public DotsGraph(IPointSource pointSource)
			: this() {
			PointSource = pointSource;
		}

		protected override Description CreateDefaultDescription() {
			return new StandartDescription();
		}

		#region Points

		public IPointSource PointSource {
			get { return (IPointSource)GetValue(PointsProperty); }
			set { SetValue(PointsProperty, value); }
		}

		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register(
			  "PointSource",
			  typeof(IPointSource),
			  typeof(DotsGraph),
			  new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsRender,
				OnPointSourceChangedCallback)
			);

		private static void OnPointSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			DotsGraph graph = (DotsGraph)d;
			graph.OnPointsChanged(e);
		}

		protected virtual void OnPointsChanged(DependencyPropertyChangedEventArgs e) {
			MakeDirty();
			InvalidateVisual();
		}

		#endregion

		#region Dot fill

		public Brush DotFill {
			get { return (Brush)GetValue(DotFillProperty); }
			set { SetValue(DotFillProperty, value); }
		}

		public static readonly DependencyProperty DotFillProperty =
			DependencyProperty.Register(
			  "DotFill",
			  typeof(Brush),
			  typeof(DotsGraph),
			  new FrameworkPropertyMetadata(Brushes.Black,
				  FrameworkPropertyMetadataOptions.AffectsRender));


		#endregion

		public double DotRadius {
			get { return (double)GetValue(DotRadiusProperty); }
			set { SetValue(DotRadiusProperty, value); }
		}

		public static readonly DependencyProperty DotRadiusProperty =
			DependencyProperty.Register(
			  "DotRadius",
			  typeof(double),
			  typeof(DotsGraph),
			  new FrameworkPropertyMetadata(5.0,
				  FrameworkPropertyMetadataOptions.AffectsRender));


		private List<Point> filteredPoints = null;

		// todo переписать
		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			Rect output = state.OutputWithMargin;

			if (PointSource == null) return;

			if (IsDirty) {
				IsDirty = false;

				List<Point> transformedPoints = PointSource.GetPoints().Transform(Viewport.Visible, output);

				// Analysis and filtering of unnecessary points
				filteredPoints = FilterPoints(transformedPoints);
			}

			DrawingGroup group = new DrawingGroup();
			using (var context = group.Open()) {
				double radius = DotRadius;
				Brush fill = DotFill;
				foreach (var point in filteredPoints) {
					context.DrawEllipse(fill, null, point, radius, radius);
				}
			}

			if (IsTranslated) {
				dc.PushTransform(new TranslateTransform(shift.X, shift.Y));
			}
			dc.DrawDrawing(group);
			if (IsTranslated) {
				dc.Pop();
			}
		}

		FrequencyFilter freqFilter = new FrequencyFilter();
		InclinationFilter inclFilter = new InclinationFilter();
		private List<Point> FilterPoints(List<Point> points) {
			freqFilter.Output = Viewport.OutputWithMargin;
			List<Point> p1 = freqFilter.Filter(points);
			List<Point> p2 = inclFilter.Filter(p1);
			return p2;
		}
	}
}
