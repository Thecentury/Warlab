using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public enum RocketHost {
		Plane,
		ZRK
	}

	public sealed class Rocket : DynamicObject, IDamageable {
		private Vector3D targetPoint;
		public Vector3D TargetPoint {
			get { return targetPoint; }
			set { targetPoint = value; }
		}

		private WarObject target;
		public WarObject Target {
			get { return target; }
			set { target = value; }
		}

		private TimeSpan timeOfExplosion;
		public TimeSpan TimeOfExposion {
			get { return timeOfExplosion; }
			set { timeOfExplosion = value; }
		}

		private double possibilityOfExplosion = 0.9;
		public double PossibilityOfExplosion {
			get { return possibilityOfExplosion; }
			set { possibilityOfExplosion = value; }
		}

		private double damage = 1;
		public double Damage {
			get { return damage; }
			set { damage = value; }
		}

		private double damageRange = Distance.FromMetres(30);
		public double DamageRange {
			get { return damageRange; }
			set { damageRange = value; }
		}

		public void Explode() {
			if (StaticRandom.NextDouble() < possibilityOfExplosion) {
				World.RocketExploded(this);
			}
			RaiseDestroyed();
		}

		private RocketHost host = RocketHost.ZRK;
		public RocketHost Host {
			get { return host; }
			set { host = value; }
		}

		protected override void UpdateImpl(WarTime warTime) {
			double distance = Position.DistanceTo(TargetPoint);
			// взрыв по времени или при достаточной близости к цели
			if (timeOfExplosion < warTime.TotalTime || distance < damageRange / 2) {
				Explode();
			}
			else {
				Vector3D shift = Orientation * warTime.ElapsedTime.TotalSeconds * Speed;
				Position += shift;
			}
		}

		#region IDamageable Members

		public double Health {
			get { return 1; }
		}

		public event EventHandler Destroyed;
		private void RaiseDestroyed() {
			if (Destroyed != null) {
				Destroyed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
