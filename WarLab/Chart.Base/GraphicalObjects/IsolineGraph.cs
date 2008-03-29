using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.GraphicalObjects;

namespace ScientificStudio.Charting.Isoline {
	public class IsolineGraph : GraphicalObject {
		private readonly IsolineCollection collection = new IsolineCollection();

		private readonly IsolinePlotter isolinePlotter = new IsolinePlotter();

		public IsolineGraph() {
			isolinePlotter.WayBeforeText = 180.0 / 5;
		}

		#region Scalar Field

		public static readonly DependencyProperty FieldProperty =
			DependencyProperty.Register(
			"Field",
			typeof(GeneralScalarField2d),
			typeof(IsolineGraph),
			new FrameworkPropertyMetadata(
				null, 
				FrameworkPropertyMetadataOptions.AffectsRender, 
				OnScalarFieldChanged));

		public GeneralScalarField2d Field {
			get { return (GeneralScalarField2d)GetValueSync(FieldProperty); }
			set { SetValueAsync(FieldProperty, value); }
		}

		private static void OnScalarFieldChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IsolineGraph isoline = (IsolineGraph)d;
			if (e.NewValue == null) {
				isoline.collection.Clear();
			}
			else {
				isoline.isolinePlotter.Process(isoline.Field, isoline.collection);
			}
			isoline.MakeDirty();
		}

		#endregion

		#region LineThickness

		public double LineThickness {
			get { return (double)GetValueSync(LineThicknessProperty); }
			set { SetValueAsync(LineThicknessProperty, value); }
		}

		public static readonly DependencyProperty LineThicknessProperty =
			DependencyProperty.Register(
			  "LineThickness",
			  typeof(double),
			  typeof(IsolineGraph),
			  new FrameworkPropertyMetadata(
				  2.0,
				  FrameworkPropertyMetadataOptions.AffectsRender,
				  OnMakeDirty)
				  );

		private static void OnMakeDirty(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			IsolineGraph graph = (IsolineGraph)d;
			graph.MakeDirty();
		}

		#endregion

		#region DrawLabelsProperty

		public bool DrawLabels {
			get { return (bool)GetValueSync(DrawLabelsProperty); }
			set { SetValueAsync(DrawLabelsProperty, value); }
		}

		public static readonly DependencyProperty DrawLabelsProperty =
			DependencyProperty.Register(
			  "DrawLabels",
			  typeof(bool),
			  typeof(IsolineGraph),
			  new FrameworkPropertyMetadata(
				  true,
				  FrameworkPropertyMetadataOptions.AffectsRender,
				  OnMakeDirty
				  ));

		#endregion

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			double thickness = LineThickness;
			Rect output = state.OutputWithMargin;

			foreach (LevelLine segment in collection.Lines) {
				StreamGeometry streamGeom = new StreamGeometry();

				using (StreamGeometryContext context = streamGeom.Open()) {
					Point startPoint = segment.StartPoint.Transform(state.Visible, output);
					List<Point> otherPoints = segment.OtherPoints.Transform(state.Visible, output);
					context.BeginFigure(startPoint, true, false);
					context.PolyLineTo(otherPoints, true, true);
				}

				streamGeom.Freeze();
				dc.DrawGeometry(null, new Pen(new SolidColorBrush(segment.Color), thickness), streamGeom);
			}

			if (state.AbortPending) {
				return;
			}

			if (DrawLabels) {
				foreach (TextLabel label in collection.Labels) {
					FormattedText text = new FormattedText(label.Text, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface("Alba"), 10, Brushes.Black);
					label.Rotation.Normalize();
					double angle = Math.Atan2(label.Rotation.Y, label.Rotation.X) * 180 / Math.PI;

					Point drawOrigin = label.Position.Transform(state.Visible, output);
					drawOrigin.Offset(-shift.X, -shift.Y);
					Point transformOrigin = drawOrigin;

					dc.PushTransform(new TranslateTransform(transformOrigin.X, transformOrigin.Y));
					dc.PushTransform(new RotateTransform(angle));
					dc.PushTransform(new TranslateTransform(-transformOrigin.X, -transformOrigin.Y));
					{
						dc.DrawText(text, drawOrigin);
					}
					dc.Pop();
					dc.Pop();
					dc.Pop();
				}
			}
		}
	}
}
