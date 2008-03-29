using System;

namespace ScientificStudio.Charting.GraphicalObjects.Descriptions
{
    public class StandartDescription : Description
    {
        public StandartDescription() { }
        public StandartDescription(string description)
        {
            if (String.IsNullOrEmpty(description))
                throw new ArgumentNullException("description");

            this.description = description;
        }

        protected override void AttachCore(IGraphicalObject graph)
        {
            if (description == null)
            {
                string str = graph.GetType().Name;
                description = str;
            }
        }

        private string description = null;

        public sealed override string Brief
        {
            get { return description; }
        }

        public sealed override string Full
        {
            get { return description; }
        }
    }
}
