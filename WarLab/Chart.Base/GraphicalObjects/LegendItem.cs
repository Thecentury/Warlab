using System.Windows.Controls;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;

namespace ScientificStudio.Charting.GraphicalObjects
{
    public abstract class LegendItem : ContentControl
    {
        protected LegendItem() { }

        protected LegendItem(Description description)
        {
            Description = description;
        }

        private Description description = null;
        public Description Description
        {
            get { return description; }
            set
            {
                description = value;
                Content = description;
            }
        }
    }
}
