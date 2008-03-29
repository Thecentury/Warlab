using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public class Legend : ContentGraph
    {
        private void InitContent()
        {
            stackPanel = new StackPanel();
            stackPanel.Margin = new Thickness(3);
        }

        public void AddLegendItem(LegendItem legendItem) {
            stackPanel.Children.Add(legendItem);
        }

        private StackPanel stackPanel;
        public Legend()
        {
            InitContent();
            Border border = new Border { BorderBrush = Brushes.Black, BorderThickness = new Thickness(1) };
            border.Child = stackPanel;
            Content = border;
        }
    }
}
