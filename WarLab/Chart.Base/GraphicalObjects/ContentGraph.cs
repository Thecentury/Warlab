﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public class ContentGraph : ContentControl, IGraphicalObject
    {
        public ContentGraph()
        {
            InitViewportService();
        }

        protected ChartPlotter ParentChartPlotter {
            get { return Parent as ChartPlotter; }
        }

        #region Viewport

        private void InitViewportService()
        {
            service.OutputChanged += service_OutputChanged;
            service.ViewportChanged += service_ViewportChanged;
            service.ViewportPropertyChanged += service_ViewportPropertyChanged;
            service.VisibleChanged += service_VisibleChanged;
        }

        private void service_VisibleChanged(object sender, RectChangedEventArgs e)
        {
            OnVisibleChanged(e.OldRect, e.NewRect);
        }

        protected virtual void OnVisibleChanged(Rect oldRect, Rect newRect) { }

        private void service_ViewportPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnViewportPropertyChanged(e);
        }

        protected virtual void OnViewportPropertyChanged(DependencyPropertyChangedEventArgs e) { }

        private void service_ViewportChanged(object sender, EventArgs e)
        {
            OnViewportChanged();
        }

        protected virtual void OnViewportChanged() { }

        private void service_OutputChanged(object sender, RectChangedEventArgs e)
        {
            OnOutputChanged(e.OldRect, e.OldRect);
        }

        protected virtual void OnOutputChanged(Rect oldRect, Rect newRect) { }

        private readonly ViewportService service = new ViewportService();

        protected Viewport2D Viewport {
            get { return service.Viewport; }
        }

        #endregion

        #region IGraphicalObject Members

        void IGraphicalObject.SetViewport(Viewport2D viewport)
        {
            service.SetViewport(viewport);
        }

        void IGraphicalObject.DetachViewport()
        {
            service.DetachViewport();
        }

        #endregion
    }
}
