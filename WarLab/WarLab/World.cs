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
using VisualListener;

namespace WarLab {
	/// <summary>
	/// Мир, в котором происходит симуляция.
	/// Содержит список объектов, позволяет узнавать время мира.
	/// 
	/// Это синглтон.
	/// </summary>
	public sealed class World : INotifyCollectionChanged {

		private static readonly World instance = new World();
		/// <summary>
		/// Возвращает синглтон - объект типа <see cref="World"/>.
		/// </summary>
		public static World Instance {
			get { return instance; }
		}

		private World() {
			objects.CollectionChanged += objects_CollectionChanged;

			RegisterAIs();

			watch.Start();
		}

		private void RegisterAIs() {
			RegisterAIForWarObject<RocketAI, Rocket>();
			RegisterAIForWarObject<RLSAI, RLS>();
		}

		private void objects_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			RaiseCollectionChanged(e);
		}

		/// <summary>
		/// Добавить объект в мир, поместив его в указанную точку.
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="position"></param>
		public void AddWarObject(WarObject obj, Vector3D position) {
			if (obj == null)
				throw new ArgumentNullException("obj");

			Type warObjectType = obj.GetType();

			if (!aiForWarObjects.ContainsKey(warObjectType))
				throw new InvalidOperationException(String.Format("Невозможно добавить объект типа {0}: для объекта типа {0} не найден ИИ. Перед добавлением объектов типа {0} необходимо выполнить метод RegisterAIForWarObject для типа {0}.", warObjectType.Name));

			Type aiType = aiForWarObjects[warObjectType];
			WarAI ai = (WarAI)Activator.CreateInstance(aiType);
			obj.SetAI(ai);

			obj.World = this;

			obj.Position = position;
			if (!isUpdating) {
				objects.Add(obj);
			}
			else {
				addedObjects.Add(obj);
			}

			IDamageable damageableObj = obj as IDamageable;
			if (damageableObj != null) {
				damageableObj.Destroyed += OnObjectDestroyed;
			}
		}

		private readonly List<WarObject> addedObjects = new List<WarObject>();

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
		/// <summary>
		/// Список всех объектов мира.
		/// </summary>
		public IEnumerable<WarObject> Objects {
			get { return objects; }
		}

		/// <summary>
		/// Список объектов мира заданного типа <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public IEnumerable<T> SelectAll<T>() {
			return objects.OfType<T>();
		}

		public IEnumerable<T> SelectAllAIs<T>() where T : WarAI {
			return objects.Where(o => o.AI is T).Select(o => o.AI as T);
		}

		/// <summary>
		/// Первый из объектов типа <typeparamref name="T"/>.
		/// Удобен, если есть уверенность, что в мире есть единственный объект данного типа.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T SelectSingle<T>() where T : WarObject {
			return objects.OfType<T>().First<T>();
		}

		public T SelectSingleAI<T>() where T : WarAI {
			return objects.First(o => o.AI is T).AI as T;
		}

		private WarTime time = new WarTime();
		/// <summary>
		/// Время мира.
		/// </summary>
		public WarTime Time {
			get { return time; }
		}

		private TimeSpan warTickTime = new TimeSpan();
		private TimeSpan warPrevTickTime = new TimeSpan();
		private TimeSpan realTickTime = new TimeSpan();
		private TimeSpan realPrevTickTime = new TimeSpan();
		private readonly Stopwatch watch = new Stopwatch();
		private double timerSpeed = 1;
		private bool isConstantDelta = true;

		private readonly TimeSpan constDelta = TimeSpan.FromMilliseconds(20);

		private bool isUpdating = false;
		/// <summary>
		/// Обновляется ли мир в данный момент.
		/// </summary>
		public bool IsUpdating { get { return isUpdating; } }

		private int tick = 0;
		/// <summary>
		/// Номер тика обновления мира.
		/// </summary>
		public int Tick {
			get { return tick; }
		}

		/// <summary>
		/// Выполняет обновление всех объектов мира.
		/// Время, прошедшее с предыдущего обновления, рассчитывается автоматически.
		/// </summary>
		public void Update() {
			tick++;
			warPrevTickTime = warTickTime;
			realPrevTickTime = realTickTime;
			realTickTime = watch.Elapsed;

			TimeSpan delta = isConstantDelta ? constDelta : realTickTime - realPrevTickTime;

			TimeSpan warDelta = TimeSpan.FromSeconds(delta.TotalSeconds * timerSpeed);
			if (!watch.IsRunning) {
				warDelta = TimeSpan.Zero;
			}

			warTickTime = warPrevTickTime + warDelta;

			time = new WarTime(warDelta, warTickTime);

			PropertyInspector.AddValueIf("World time", warTickTime.TotalSeconds, tick % 12 == 0);

			if (time.ElapsedTime.TotalMilliseconds == 0)
				return;

			isUpdating = true;
			foreach (var obj in objects) {
				obj.UpdateAI(time);
			}

			foreach (var obj in objects) {
				obj.ExecuteAICommands();
			}

			foreach (var obj in objects) {
				obj.UpdateSelf(time);
			}
			isUpdating = false;

			foreach (var obj in addedObjects) {
				objects.Add(obj);
			}
			addedObjects.Clear();

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
					if (distance < rocket.DamageRange) {
						double damage = (rocket.DamageRange - distance) / rocket.DamageRange * rocket.Damage;
						rocketDamageable.MakeDamage(damage);
					}
				}
			}
		}

		/// <summary>
		/// Взорвать бомбу в указанной точке.
		/// </summary>
		/// <param name="position">Место взрыва бомбы.</param>
		/// <param name="damage">Максимальные повреждения, наносимые бомбой.</param>
		/// <param name="damageRange">Радиус зоны поражения.</param>
		public void ExplodeBomb(Vector3D position, double damage, double damageRange) {
			foreach (var item in SelectAll<StaticObject>()) {
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

		/// <summary>
		/// Зарегистрировать ИИ для типа объектов мира.
		/// Говорит миру, что объекты типа <paramref name="TAI"/> служат ИИ для объектов типа <paramref name="TWarObject"/>
		/// </summary>
		/// <typeparam name="TAI"></typeparam>
		/// <typeparam name="TWarObject"></typeparam>
		public void RegisterAIForWarObject<TAI, TWarObject>()
			where TAI : WarAI
			where TWarObject : WarObject {

			Type warType = typeof(TWarObject);

			Type aiType = typeof(TAI);

			aiForWarObjects.Add(typeof(TWarObject), typeof(TAI));
		}

		public ITimeControl GetTimeControl() {
			return new WorldTimeControl(this);
		}

		private sealed class WorldTimeControl : ITimeControl {
			private readonly World world;

			public WorldTimeControl(World world) {
				this.world = world;
			}

			#region ITimeControl Members

			public double Speed {
				get { return world.timerSpeed; }
				set {
					Verify.IsFinite(value);
					world.timerSpeed = value;
				}
			}

			public void Start() {
				world.watch.Start();
			}

			public void Stop() {
				world.watch.Stop();
			}

			public bool IsRunning {
				get { return world.watch.IsRunning; }
			}

			#endregion
		}
	}
}
