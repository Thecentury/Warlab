using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificStudio.Charting
{
    public interface IGraphicalObject
    {
        void SetViewport(Viewport2D viewport);
        void DetachViewport();
    }
}
