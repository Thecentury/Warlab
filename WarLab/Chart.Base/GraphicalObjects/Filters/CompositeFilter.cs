using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects.Filters {
	public sealed class CompositeFilter : IFilter {
		public CompositeFilter() { }

		public CompositeFilter(params IFilter[] filters) {
			if (filters == null)
				throw new ArgumentNullException("filters");

			this.filters = new List<IFilter>(filters);
		}

		private readonly List<IFilter> filters = new List<IFilter>();

		#region IFilter Members

		public List<Point> Filter(List<Point> points) {
			foreach (var filter in filters) {
				points = filter.Filter(points);
			}
			return points;
		}

		#endregion
	}
}
