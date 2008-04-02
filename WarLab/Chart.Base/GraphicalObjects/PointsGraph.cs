using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Windows.Shapes;
using ScientificStudio.Charting.GraphicalObjects;
using System.Collections.Generic;
using System.Globalization;
using ScientificStudio.Charting.GraphicalObjects.Filters;
using ScientificStudio.Charting.PointSources;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public class PointsGraph : GraphicalObject
    {
        public PointsGraph()
        {
            Legend.SetShowInLegend(this, true);
        }

        protected override Description CreateDefaultDescription()
        {
            return new PenDescription();
        }

        #region Points

        public IPointSource PointSource
        {
            get { return (IPointSource)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
              "PointSource",
              typeof(IPointSource),
              typeof(PointsGraph),
              new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnPointSourceChangedCallback)
            );

        private static void OnPointSourceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointsGraph graph = (PointsGraph)d;
            /*
            graph.DetachPointSource(e.OldValue as IPointSource);
            graph.AttachPointSource(e.NewValue as IPointSource);
             */
            graph.OnPointsChanged(e);
        }

        /*
        private void AttachPointSource(IPointSource source) {
            if (source != null)
            {
                source.PointsChanged += source_PointsChanged;
            }
        }

        private void DetachPointSource(IPointSource source)
        {
            if (source != null)
            {
                source.PointsChanged -= source_PointsChanged;
            }
        }
        
        private void source_PointsChanged(object sender, EventArgs e)
        {
            // todo uncomment this
            //MakeDirty();
            //InvalidateVisual();
        }
         */


        protected virtual void OnPointsChanged(DependencyPropertyChangedEventArgs e)
        {
            MakeDirty();
            InvalidateVisual();
        }

        #endregion

        #region Pen

        public Pen LinePen
        {
            get { return (Pen)GetValue(LinePenProperty); }
            set { SetValue(LinePenProperty, value); }
        }

        public static readonly DependencyProperty LinePenProperty =
            DependencyProperty.Register(
            "LinePen",
            typeof(Pen),
            typeof(PointsGraph),
            new FrameworkPropertyMetadata(
                new Pen(Brushes.Blue, 1),
                FrameworkPropertyMetadataOptions.AffectsRender
                ));

        #endregion

        private FakePointList filteredPoints = null;

        // todo переписать
        protected override void OnRenderCore(DrawingContext dc, RenderState state)
        {
            Rect output = state.OutputWithMargin;

            if (this.PointSource == null) return;

            if (IsDirty)
            {
                IsDirty = false;

                List<Point> transformedPoints = PointSource.GeneratePoints().Transform(Viewport.Visible, output);

                // Analysis and filtering of unnecessary points
                filteredPoints = new FakePointList(FilterPoints(transformedPoints), output.Left, output.Right);
            }
            else
            {
                double left = output.Left;
                double right = output.Right;
                left -= shift.X;
                right -= shift.X;

                filteredPoints.SetBorders(left, right);
            }

            if (filteredPoints.HasPoints)
            {
                StreamGeometry geometry = new StreamGeometry();
                StreamGeometryContext context = geometry.Open();
                context.BeginFigure(filteredPoints.StartPoint, false, false);
                context.PolyLineTo(filteredPoints, true, true);
                context.Close();
                geometry.Freeze();

                Brush brush = null;
                Pen pen = LinePen;

                if (IsTranslated)
                {
                    dc.PushTransform(new TranslateTransform(shift.X, shift.Y));
                }
                dc.DrawGeometry(brush, pen, geometry);
                if (IsTranslated)
                {
                    dc.Pop();
                }
#if DEBUG
				FormattedText text = new FormattedText(filteredPoints.Count.ToString(),
					CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
					new Typeface("Arial"), 12, Brushes.Black);
				dc.DrawText(text, Viewport.OutputWithMargin.GetCenter());
#endif
            }
        }

        FrequencyFilter freqFilter = new FrequencyFilter();
        InclinationFilter inclFilter = new InclinationFilter();
        private List<Point> FilterPoints(List<Point> points)
        {
            freqFilter.Output = Viewport.OutputWithMargin;
            List<Point> p1 = freqFilter.Filter(points);
            List<Point> p2 = inclFilter.Filter(p1);
            return p2;
        }
    }
}
