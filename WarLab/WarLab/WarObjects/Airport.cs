using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
				fromPrevLaunch = TimeSpan.Zero;
			}
		}

		public bool CanLaunch<T>() where T : Plane {
			if (fromPrevLaunch != TimeSpan.Zero) return false;

			var planeInfo = SearchReadyPlane<T>();
			return planeInfo != null;
		}

		private AirportPlaneInfo SearchReadyPlane<T>() where T : Plane {
			return planes.FirstOrDefault(info => info.Plane is T && info.State == AirportPlaneState.ReadyToFly);
		}

		public T LaunchPlane<T>() where T : Plane {
			if (fromPrevLaunch != TimeSpan.Zero)
				throw new InvalidOperationException("Нельзя запускать самолет - еще слишком рано!");

			var planeInfo = SearchReadyPlane<T>();

			if (planeInfo != null) {
				planeInfo.State = AirportPlaneState.InAir;
				T plane = (T)planeInfo.Plane;

				World.AddWarObject(plane, Position);

				LaunchPlaneCore(plane);

				fromPrevLaunch = planeLaunchDelay;

				return plane;
			}

			return null;
		}

		protected virtual void LaunchPlaneCore(Plane plane) { }

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

			var planeInfo = planes.Find(info => info.Plane == plane);
			planeInfo.State = AirportPlaneState.Refueling;
			BeginReloadAndRefuel(plane);
			planeInfo.State = AirportPlaneState.ReadyToFly;
		}
	}
}
