using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScientificStudio.Charting.GraphicalObjects
{
    [ValueConversion(typeof(Point), typeof(double))]
    public sealed class CoordinateConverter : FrameworkContentElement, IValueConverter
    {

        public ItemsGraph Graph
        {
            get { return (ItemsGraph)GetValue(GraphProperty); }
            set { SetValue(GraphProperty, value); }
        }

        public static readonly DependencyProperty GraphProperty =
            DependencyProperty.Register(
            "Graph",
            typeof(ItemsGraph),
            typeof(CoordinateConverter), new FrameworkPropertyMetadata(
                null,
                OnGraphChanged
                ));

        private static void OnGraphChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinateConverter c = (CoordinateConverter)d;
            c.Viewport = c.Graph.Viewport;
        }


        public Viewport2D Viewport
        {
            get { return (Viewport2D)GetValue(ViewportProperty); }
            set { SetValue(ViewportProperty, value); }
        }

        public static readonly DependencyProperty ViewportProperty =
            DependencyProperty.Register(
            "Viewport", 
            typeof(Viewport2D), 
            typeof(CoordinateConverter), new UIPropertyMetadata(null));


        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Point visiblePt = (Point)value;
            if (Viewport == null)
            {
                return 0;
            }
            Point screenPt = visiblePt.Transform(Viewport.Visible, Viewport.OutputWithMargin);
            string param = (string)parameter;
            if (param == "X")
            {
                return screenPt.X;
            }
            else
            {
                return screenPt.Y;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
