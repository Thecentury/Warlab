using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using WarLab.AI;
using System.Diagnostics;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;
using WarLab.WarObjects;

namespace WarLab {
	public sealed class World : INotifyCollectionChanged {
		public World() {
			objects.CollectionChanged += objects_CollectionChanged;

			RegisterAIs();
		}

		private void RegisterAIs() {
			RegisterAIForWarObject<ImprovedRocketAI, Rocket>();
			RegisterAIForWarObject<RLSAI, RLS>();
		}

		private void objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseCollectionChanged(e);
		}

		public void AddWarObject(WarObject obj, Vector3D position) {
			Type warObjectType = obj.GetType();

			if (!aiForWarObjects.ContainsKey(warObjectType))
				throw new InvalidOperationException(String.Format("Невозможно добавить объект типа {0}: для объекта типа {0} не найден ИИ. Перед добавлением объектов типа {0} необходимо выполнить метод RegisterAIForWarObject для типа {0}.", warObjectType.Name));

			Type aiType = aiForWarObjects[warObjectType];
			WarAI ai = (WarAI)Activator.CreateInstance(aiType);
			obj.SetAI(ai);

			obj.World = this;

			obj.Position = position;
			objects.Add(obj);

			IDamageable damageableObj = obj as IDamageable;
			if (damageableObj != null) {
				damageableObj.Destroyed += OnObjectDestroyed;
			}
		}

		private readonly List<WarObject> destroyedObjects = new List<WarObject>();
		private void OnObjectDestroyed(object sender, EventArgs e) {
			IDamageable damageable = sender as IDamageable;
			destroyedObjects.Add(sender as WarObject);
			damageable.Destroyed -= OnObjectDestroyed;

			Debug.WriteLine(String.Format("{0} was destroyed", sender.GetType().Name));
		}

		public event ObjectDestroyedEventHandler ObjectDestroyed;
		private void RaiseObjectDestroyed(WarObject destroyedObject) {
			if (ObjectDestroyed != null) {
				ObjectDestroyed(this, new ObjectDestroyedEventArgs(destroyedObject));
			}
		}

		private readonly ObservableCollection<WarObject> objects = new ObservableCollection<WarObject>();
		public IEnumerable<WarObject> Objects {
			get { return objects; }
		}

		public IEnumerable<T> SelectAll<T>() where T : WarObject {
			return objects.OfType<T>();
		}

		public void Update(WarTime time) {
			if (time.ElapsedTime.TotalMilliseconds == 0)
				return;

			foreach (var obj in objects) {
				obj.UpdateAI(time);
			}

			foreach (var obj in objects) {
				obj.ExecuteAICommands();
			}

			foreach (var obj in objects) {
				obj.UpdateSelf(time);
			}

			foreach (var obj in destroyedObjects) {
				objects.Remove(obj);
				RaiseObjectDestroyed(obj);
			}
			destroyedObjects.Clear();
		}

		internal void RocketExploded(Rocket rocket) {
			Vector3D targetPos = rocket.TargetPoint;
			foreach (var item in SelectAll<DynamicObject>()) {
				IRocketDamageable rocketDamageable = item as IRocketDamageable;
				if (rocketDamageable != null) {
					double distance = MathHelper.Distance(item.Position, targetPos);
					Debug.WriteLine(distance);
					if (distance < rocket.DamageRange) {
						double damage = (rocket.DamageRange - distance) / rocket.DamageRange * rocket.Damage;
						rocketDamageable.MakeDamage(damage);
					}
				}
			}
		}

		public void BombExploded(Vector3D position, double damage, double damageRange) {
			foreach (var item in SelectAll<DynamicObject>()) {
				IBombDamageable bombDamageable = item as IBombDamageable;
				if (bombDamageable != null) {
					double distance = MathHelper.Distance(item.Position, position);
					if (distance < damageRange) {
						double realDamage = (damageRange - distance) / damageRange * damage;
						bombDamageable.MakeDamage(realDamage);
					}
				}
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
