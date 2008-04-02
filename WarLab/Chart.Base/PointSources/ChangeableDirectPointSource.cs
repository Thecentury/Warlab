using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ScientificStudio.Charting.PointSources {
	public class ChangeableDirectPointSource : PointSource1dBase {

		#region Ctors

		private void Init() {
			points.CollectionChanged += points_CollectionChanged;
		}

		public ChangeableDirectPointSource() {
			Init();
		}

		public ChangeableDirectPointSource(IEnumerable<Point> points)
			: this() {

			if (points == null)
				throw new ArgumentNullException("points");

			foreach (var p in points) {
				this.points.Add(p);
			}
		}

		#endregion

		private readonly ObservableCollection<Point> points = new ObservableCollection<Point>();
		public ObservableCollection<Point> Points {
			get { return points; }
		}

		private bool notifyCollectionChanges = true;
		public void AppendMany(IEnumerable<Point> points) {
			notifyCollectionChanges = false;
			foreach (var p in points) {
				this.points.Add(p);
			}
			notifyCollectionChanges = true;
			RaisePointsChanged();
		}

		private void points_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (notifyCollectionChanges) {
				RaisePointsChanged();
			}
		}

		protected override ICollection<Point> GetPointsCore() {
			bounds = points.GetBounds();

			return points;
		}

		private Rect bounds = Rect.Empty;
		public override Rect Bounds {
			get { return bounds; }
		}

		protected override Freezable CreateInstanceCore() {
			return new ChangeableDirectPointSource();
		}
	}
}
