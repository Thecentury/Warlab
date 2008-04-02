using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.PointSources
{
    public interface IPointSource
    {
        List<Point> GeneratePoints();
    }
}
