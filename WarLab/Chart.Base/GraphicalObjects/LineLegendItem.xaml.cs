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
using ScientificStudio.Charting.GraphicalObjects.Descriptions;

namespace ScientificStudio.Charting.GraphicalObjects
{
    /// <summary>
    /// Interaction logic for LineLegendItem.xaml
    /// </summary>
    public partial class LineLegendItem : LegendItem
    {
        public LineLegendItem()
        {
            InitializeComponent();
        }

        public LineLegendItem(Description description) : base(description)
        {
            InitializeComponent();
        }
    }
}
