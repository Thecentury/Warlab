using System;
using System.Windows;

namespace ScientificStudio.Charting
{
    public sealed class RectChangedEventArgs : EventArgs
    {
        public RectChangedEventArgs(Rect oldRect, Rect newRect)
        {
            this.oldRect = oldRect;
            this.newRect = newRect;
        }

        private readonly Rect oldRect;
        public Rect OldRect
        {
            get { return oldRect; }
        }

        private readonly Rect newRect;
        public Rect NewRect {
            get { return newRect; }
        }
    }
}
