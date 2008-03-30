using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WarLab {
	public sealed class World : INotifyCollectionChanged {
		private World() {
			objects.CollectionChanged += objects_CollectionChanged;
		}

		private void objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseCollectionChanged(e);
		}

		private static readonly World instance = new World();
		public static World Instance {
			get { return instance; }
		}

		public void AddWarObject(WarObject obj, Vector3D position) {
			obj.Position = position;
			objects.Add(obj);
		}

		private readonly ObservableCollection<WarObject> objects = new ObservableCollection<WarObject>();
		public IEnumerable<WarObject> Objects {
			get { return objects; }
		}

		public IEnumerable<T> SelectAll<T>() where T : WarObject {
			return objects.OfType<T>();
		}

		public void Update(WarTime time) {
			foreach (var obj in objects) {
				obj.Update(time);
			}
		}

		#region INotifyCollectionChanged Members

		private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if (CollectionChanged != null) {
				CollectionChanged(this, e);
			}
		}

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		#endregion
	}
}
