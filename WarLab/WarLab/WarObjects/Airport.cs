using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab.WarObjects {
	public delegate Plane PlaneCreator(int counter);

	public abstract class Airport : StaticObject {
		private readonly List<AirportPlaneInfo> planes = new List<AirportPlaneInfo>();
		public List<AirportPlaneInfo> Planes {
			get { return planes; }
		}

		public List<T> PlanesOfType<T>() where T : Plane {
			return planes.Where(info => info.Plane is T).
				Select(info => info.Plane as T).ToList();
		}

		private void VerifyPlaneType(Plane plane) {
			if (!VerifyPlaneTypeCore(plane))
				throw new ArgumentException("Этот аэродром не может нести самолеты типа " + plane.GetType().Name);
		}

		protected virtual bool VerifyPlaneTypeCore(Plane plane) {
			return true;
		}

		public void AddPlanes(PlaneCreator creator, int planesNum) {
			if (creator == null)
				throw new ArgumentNullException("creator");
			Verify.IsNonNegative(planesNum);

			Plane[] tempPlanes = new Plane[planesNum];
			for (int i = 0; i < planesNum; i++) {
				tempPlanes[i] = creator(i);
			}

			AddPlanes(tempPlanes);
		}

		public void AddPlanes(params Plane[] planes) {
			if (planes == null)
				throw new ArgumentNullException("planeTypes");

			foreach (var plane in planes) {
				if (plane == null)
					throw new ArgumentNullException("plane");

				VerifyPlaneType(plane);

				plane.Airport = this;

				AirportPlaneInfo planeInfo = new AirportPlaneInfo(plane);
				this.planes.Add(planeInfo);
			}
		}

		private TimeSpan fromPrevLaunch = new TimeSpan();

		private TimeSpan planeLaunchDelay = TimeSpan.FromSeconds(5);
		public TimeSpan PlaneLaunchDelay {
			get { return planeLaunchDelay; }
			set { planeLaunchDelay = value; }
		}

		protected override void UpdateImpl(WarTime time) {
			if (fromPrevLaunch > TimeSpan.Zero) {
				fromPrevLaunch -= time.ElapsedTime;
			}
			else {
				// готов к запуску самолета.
				fromPrevLaunch = TimeSpan.Zero;
			}
			if (fromPrevLaunch < TimeSpan.Zero) {
				fromPrevLaunch = TimeSpan.Zero;
			}

			if (fromPrevLaunch == TimeSpan.Zero && planesToLaunch.Count != 0) {
				Plane plane = planesToLaunch.Dequeue();
				World.AddObject(plane, Position);
				fromPrevLaunch = planeLaunchDelay;
			}
		}

		public bool CanLaunch<T>() where T : Plane {
			if (fromPrevLaunch > TimeSpan.Zero) return false;

			var planeInfo = SearchReadyPlane<T>();
			return planeInfo != null;
		}

		private AirportPlaneInfo SearchReadyPlane<T>() where T : Plane {
			return planes.
				FirstOrDefault(info => info.Plane is T && info.State == AirportPlaneState.ReadyToFly);
		}

		public T LaunchPlane<T>() where T : Plane {
			return LaunchPlane<T>(true);
		}

		private double flightHeight = Distance.FromKilometres(1);
		public double FlightHeight {
			get { return flightHeight; }
			set { flightHeight = value; }
		}

		private T LaunchPlane<T>(bool addToWorld) where T : Plane {
			if (addToWorld && fromPrevLaunch != TimeSpan.Zero)
				throw new InvalidOperationException("Нельзя запускать самолет - еще слишком рано!");

			var planeInfo = SearchReadyPlane<T>();

			if (planeInfo != null) {
				planeInfo.State = AirportPlaneState.InAir;
				T plane = (T)planeInfo.Plane;

				if (addToWorld) {
					World.AddObject(plane, new Vector3D(Position, flightHeight));
					fromPrevLaunch = planeLaunchDelay;
				}
				else {
					World.PreAddInit(plane);
				}

				plane.Destroyed += OnPlaneDestroyed;

				return plane;
			}

			return null;
		}

		private void OnPlaneDestroyed(object sender, EventArgs e) {
			Plane p = (Plane)sender;
			p.Destroyed -= OnPlaneDestroyed;

			var planeInfo = planes.Find(pi => pi.Plane == p);
			planeInfo.State = AirportPlaneState.Dead;
		}

		private readonly Queue<Plane> planesToLaunch = new Queue<Plane>();
		public T QueueLaunchPlane<T>() where T : Plane {
			T plane = LaunchPlane<T>(false);

			if (plane != null) {
				planesToLaunch.Enqueue(plane);
			}

			return plane;
		}

		public bool QueueLaunchPlane<T>(T plane) where T : Plane {
			Verify.IsTrue(plane.Airport == this);

			AirportPlaneInfo info = planes.Find(pi => pi.Plane == plane);
			bool res = info.State == AirportPlaneState.ReadyToFly || info.State == AirportPlaneState.InAir;

			if (res && !(info.State == AirportPlaneState.InAir)) {
				planesToLaunch.Enqueue(plane);
			}

			return res;
		}

		/// <summary>
		/// Заправить и перезарядить самолет.
		/// </summary>
		/// <param name="plane">Самолет</param>
		public void BeginReloadAndRefuel(Plane plane) {
			Verify.IsTrue(plane.Airport == this);

			plane.Refuel();
			plane.Reload();
		}

		public void LandPlane(Plane plane) {
			if (plane == null)
				throw new ArgumentNullException("plane");

			Verify.IsTrue(plane.Airport == this);

			World.RemoveWarObject(plane);

			plane.Destroyed -= OnPlaneDestroyed;

			var planeInfo = planes.Find(info => info.Plane == plane);
			planeInfo.State = AirportPlaneState.Refueling;
			BeginReloadAndRefuel(plane);
			planeInfo.State = AirportPlaneState.ReadyToFly;
		}

		public void ReloadAndRefuelInAir(Plane plane) {
			if (plane == null)
				throw new ArgumentNullException("plane");
	
			Verify.IsTrue(plane.Airport == this);
			BeginReloadAndRefuel(plane);
		}
	}
}
