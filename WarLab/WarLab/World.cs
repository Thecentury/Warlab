using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WarLab.AI;
using System.Diagnostics;

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

			if (!aiForWarObjects.ContainsKey(warObjectType))
				throw new InvalidOperationException(String.Format("Невозможно добавить объект типа {0}: для объекта типа {0} не найден ИИ. Перед добавлением объектов типа {0} необходимо выполнить метод RegisterAIForWarObject для типа {0}.", warObjectType.Name));

			Type aiType = aiForWarObjects[warObjectType];
			WarAI ai = (WarAI)Activator.CreateInstance(aiType);
			obj.SetAI(ai);

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

		int counter = 0;
		public void Update(WarTime time) {
			counter++;
			bool measureDurationOfCall = false && counter % 50 == 0;
			int startTime = 0;
			if (measureDurationOfCall)
				startTime = Environment.TickCount;

			foreach (var obj in objects) {
				obj.UpdateAI(time);
			}

			foreach (var obj in objects) {
				obj.ExecuteAICommands();
			}

			foreach (var obj in objects) {
				obj.UpdateSelf(time);
			}

			if (measureDurationOfCall) {
				int duration = Environment.TickCount - startTime;
				Debug.WriteLine(String.Format("Duration = {0} ms", duration));
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
			bool valid = ((ControlsAttribute[])attrs).Any<ControlsAttribute>(attr => attr.ControllsType.IsAssignableFrom(warType));
			if (!valid)
				throw new InvalidOperationException(String.Format("Объекты типа {0} не может управлять объектами типа {1}, что следует из аттрибутов, навешеных на тип {0}", aiType.Name, warType.Name));

			aiForWarObjects.Add(typeof(TWarObject), typeof(TAI));
		}
	}
}
