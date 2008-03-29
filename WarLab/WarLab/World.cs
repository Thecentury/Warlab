using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace WarLab {
	public sealed class World {
		private static readonly World instance = new World();
		public static World Instance {
			get { return instance; }
		}

		private readonly ObservableCollection<WarObject> objects = new ObservableCollection<WarObject>();
		public ObservableCollection<WarObject> Objects {
			get { return objects; }
		}

		public IEnumerable<T> SelectAll<T>() where T : WarObject {
			return objects.OfType<T>();
		}

		public void Update(WarTime time) {
			throw new NotImplementedException();
		}
	}
}
