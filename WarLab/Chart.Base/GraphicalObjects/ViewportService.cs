using System;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public sealed class ViewportService : IGraphicalObject
    {
        private Viewport2D viewport = null;
        public Viewport2D Viewport
        {
            get { return viewport; }
        }

        #region IGraphicalObject Members

        public void SetViewport(Viewport2D viewport)
        {
            DetachViewport(this.viewport);
            this.viewport = viewport;
            AttachViewport(viewport);
            RaiseViewportChanged();
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

        private void OnViewportRectChanged(Viewport2D viewport, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property == Viewport2D.OutputProperty)
            {
                RaiseOutputChanged(e);
            }
            else if (e.Property == Viewport2D.VisibleProperty)
            {
                RaiseVisibleChanged(e);
            }
            else
            {
                // other rects changed are now not interesting for us
            }
            RaiseViewportPropertyChanged(e);
        }

        public void DetachViewport()
        {
            DetachViewport(viewport);
        }

        public event EventHandler<RectChangedEventArgs> VisibleChanged;
        private void RaiseVisibleChanged(DependencyPropertyChangedEventArgs e)
        {
            RectChangedEventArgs rectEA = FromDependencyPropertyChangedEventArgs(e);
            if (VisibleChanged != null)
            {
                VisibleChanged(this, rectEA);
            }
        }

        public event EventHandler<RectChangedEventArgs> OutputChanged;
        private void RaiseOutputChanged(DependencyPropertyChangedEventArgs e)
        {
            RectChangedEventArgs rectEA = FromDependencyPropertyChangedEventArgs(e);
            if (OutputChanged != null)
            {
                OutputChanged(this, rectEA);
            }
        }

        public event DependencyPropertyChangedEventHandler ViewportPropertyChanged;
        private void RaiseViewportPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ViewportPropertyChanged != null)
            {
                ViewportPropertyChanged(this, e);
            }
        }

        public event EventHandler ViewportChanged;
        private void RaiseViewportChanged()
        {
            if (ViewportChanged != null)
            {
                ViewportChanged(this, EventArgs.Empty);
            }
        }

        private static RectChangedEventArgs FromDependencyPropertyChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            return new RectChangedEventArgs((Rect)e.OldValue, (Rect)e.NewValue);
        }

        #endregion
    }
}
