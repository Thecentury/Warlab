using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
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

		public delegate Plane PlaneCreator(int counter);

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

		protected T LaunchPlane<T>() where T : Plane {
			var planeInfo = planes.FirstOrDefault(info => info.Plane is T && info.State == AirportPlaneState.ReadyToFly);
			if (planeInfo != null) {
				planeInfo.State = AirportPlaneState.InAir;
				T plane = (T)planeInfo.Plane;

				World.AddWarObject(plane, Position);

				return plane;
			}

			return null;
		}

		/// <summary>
		/// Заправить и перезарядить самолет.
		/// </summary>
		/// <param name="plane">Самолет</param>
		public void ReloadAndRefuel(Plane plane) {
			Verify.IsTrue(plane.Airport == this);

			plane.Refuel();
			plane.Reload();
		}
	}
}
