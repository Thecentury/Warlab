using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScientificStudio.Charting.GraphicalObjects
{
    /// <summary>
    /// Interaction logic for Legend.xaml
    /// </summary>
    public partial class Legend : ContentGraph
    {
        public Legend()
        {
            InitializeComponent();
            Canvas.SetTop(this, 5);
            Canvas.SetRight(this, 5);
        }

        public void AddLegendItem(LegendItem legendItem)
        {
            stackPanel.Children.Add(legendItem);
        }

        protected override void OnViewportChanged()
        {
            PopulateLegend();
        }

        private PopulationMethod populationMethod = PopulationMethod.Auto;
        public PopulationMethod PopulationMethod
        {
            get { return populationMethod; }
            set
            {
                if (populationMethod != value)
                {
                    populationMethod = value;
                    // todo доделать
                    PopulateLegend();
                }
            }
        }

        private void PopulateLegend()
        {
            if (populationMethod == PopulationMethod.Auto)
            {
                // todo доделать
                foreach (IGraphicalObject graph in ParentChartPlotter.GraphChildren)
                {
                    // todo подписываться на события об изменении коллекции у plotter'а
                    GraphicalObject d = graph as GraphicalObject;
                    if (d != null && GetShowInLegend(d))
                    {
                        AddLegendItem(new LineLegendItem(d.Description));
                    }
                }
            }
        }

        #region ShowInLegend attached dependency property

        public static bool GetShowInLegend(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowInLegendProperty);
        }

        public static void SetShowInLegend(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowInLegendProperty, value);
        }

        public static readonly DependencyProperty ShowInLegendProperty =
            DependencyProperty.RegisterAttached(
            "ShowInLegend",
            typeof(bool),
            typeof(Legend), new UIPropertyMetadata(
                false
                ));

        #endregion
    }

    public enum PopulationMethod
    {
        Auto,
        Manual
    }
}
