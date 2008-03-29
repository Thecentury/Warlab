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
