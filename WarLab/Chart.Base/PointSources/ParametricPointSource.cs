using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.PointSources
{
    public sealed class ParametricPointSource : IPointSource
    {
        public double StartT { get; set; }
        public double EndT { get; set; }
        public int Number { get; set; }
        public Func<double, Point> F { get; set; }

        #region IPointSource Members

        public List<Point> GeneratePoints()
        {
            List<Point> pts = new List<Point>(Number);
            double step = (EndT - StartT) / Number;
            double start = StartT;
            for (int i = 0; i <= Number + 1; i++)
            {
                pts.Add(F(start + step * i));
            }
            return pts;
        }

        #endregion
    }
}
