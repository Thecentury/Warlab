using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;

namespace ScientificStudio.Charting.PointSources
{
    public interface INotifyPointsChanged
    {
        event EventHandler PointsChanged;
    }

    public class PointSource1d : Animatable, INotifyPointsChanged, IPointSource
    {

        public double Time
        {
            get { return (double)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(
            "Time",
            typeof(double),
            typeof(PointSource1d),
            new UIPropertyMetadata(0.0, OnTimeChanged));

        private static void OnTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PointSource1d p = (PointSource1d)d;
            p.RaisePointsChanged();
        }

        private Func<double, double, double> f_time = null;
        public Func<double, double, double> F_Time
        {
            get { return f_time; }
            set { f_time = value; }
        }

        private Func<double, double> f = null;
        public Func<double, double> F
        {
            get { return f; }
            set { f = value; }
        }

        public double Start { get; set; }
        public double Duration { get; set; }
        public int Number { get; set; }

        public List<Point> GeneratePoints()
        {
            List<Point> pts = new List<Point>();

            Func<double, double> f = this.f;
            if (f == null)
            {
                double time = Time;
                f = x => f_time(x, time);
            }

            double duration = Duration;
            double start = Start;
            int number = Number;

            double step = duration / (number);
            for (int i = 0; i < number + 1; i++)
            {
                double x = start + step * i;
                double y = f(x);
                pts.Add(new Point(x, y));
            }
            return pts;
        }

        public event EventHandler PointsChanged;
        private void RaisePointsChanged()
        {
            if (PointsChanged != null)
            {
                PointsChanged(this, EventArgs.Empty);
            }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new PointSource1d();
        }
    }
}
