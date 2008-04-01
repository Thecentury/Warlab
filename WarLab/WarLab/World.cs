using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WarLab.AI;

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
			Type warObjectType = obj.GetType();

			if (aiForWarObjects.ContainsKey(warObjectType)) {
				Type aiType = aiForWarObjects[warObjectType];
				WarAI ai = (WarAI)Activator.CreateInstance(aiType);
				obj.SetAI(ai);
			}

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

			foreach (var obj in objects) {
				obj.ExecuteAICommands();
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

		private readonly Dictionary<Type, Type> aiForWarObjects = new Dictionary<Type, Type>();

		public void RegisterAIForWarObject<TAI, TWarObject>()
			where TAI : WarAI
			where TWarObject : WarObject {

			Type warType = typeof(TWarObject);

			Type aiType = typeof(TAI);
			var attrs = aiType.GetCustomAttributes(typeof(ControlsAttribute), true);
			bool valid = ((ControlsAttribute[])attrs).Any<ControlsAttribute>(attr => attr.ControllsType == warType);
			if (!valid)
				throw new InvalidOperationException(String.Format("Тип {0} не может управлять объектами типа {1}, что следует из аттрибутов, навешенных на тип {0}", aiType.Name, warType.Name));

			aiForWarObjects.Add(typeof(TWarObject), typeof(TAI));
		}
	}
}
