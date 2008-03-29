using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public class ItemsGraph : ItemsControl, IGraphicalObject
    {
        public ItemsGraph() { }

        #region Points

        public List<Point> Points
        {
            get { return (List<Point>)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(
              "Points",
              typeof(List<Point>),
              typeof(ItemsGraph),
              new FrameworkPropertyMetadata(
                null,
                FrameworkPropertyMetadataOptions.AffectsRender,
                OnPointsChangedCallback)
            );

        private static void OnPointsChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsGraph graph = (ItemsGraph)d;
            graph.OnPointsChanged(e);
        }

        protected virtual void OnPointsChanged(DependencyPropertyChangedEventArgs e)
        {
            ItemsSource = (IEnumerable)e.NewValue;
            //MakeDirty();
        }

        private CoordinateConverter converter;
        public CoordinateConverter Converter
        {
            get
            {
                if (converter == null)
                {
                    converter = new CoordinateConverter { Viewport = viewport };
                }
                return converter;
            }
        }

        #endregion

        private Viewport2D viewport = null;
        public Viewport2D Viewport
        {
            get { return viewport; }
        }

        void IGraphicalObject.SetViewport(Viewport2D viewport)
        {
            DetachViewport(this.viewport);
            this.viewport = viewport;
            AttachViewport(viewport);
            Resources.Add("conv", Converter);

            // todo very-very stupid hack
            Width = 800;// viewport.Output.Width;
            Height = 650;// viewport.Output.Height;
            //OnViewportChanged();
        }

        void IGraphicalObject.DetachViewport()
        {
            DetachViewport(viewport);
        }

        private void DetachViewport(Viewport2D viewport)
        {
            if (viewport != null)
            {
                viewport.RectChanged -= OnViewportRectChanged;
            }
        }

        private void AttachViewport(Viewport2D viewport)
        {
            if (viewport == null)
                throw new ArgumentNullException("viewport");

            viewport.RectChanged += OnViewportRectChanged;
        }

        protected virtual void OnViewportRectChanged(Viewport2D viewport, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
