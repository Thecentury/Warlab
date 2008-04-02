using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	public abstract class Plane : DynamicObject, IRocketDamageable {
		protected sealed override void UpdateImpl(WarTime warTime) {
			UpdateCore(warTime);

			Vector3D shift = Orientation * warTime.ElapsedTime.TotalSeconds * Speed;
			FuelLeft -= shift.Length;
			Position += shift;
		}

		/// <summary>
		/// Метод, который будет вызываться каждый такт и именно в котором нужно реализовывать ИИ.
		/// </summary>
		/// <param name="warTime">The war time.</param>
		protected abstract void UpdateCore(WarTime warTime);

		private double fuelLeft;
		public double FuelLeft {
			get { return fuelLeft; }
			internal set {
				fuelLeft = value;
				if (fuelLeft < 0.01) {
					RaiseDead();
				}
			}
		}

		#region IRocketDamageable Members

		public void MakeDamage(double damage) {
			Verify.DoubleIsPositive(damage);

			health -= damage;
			if (health <= 0.01) {
				RaiseDead();
			}
		}

		#endregion

		#region IDamageable Members

		private double health = 1;
		public double Health {
			get { return health; }
		}

		public event EventHandler Dead;
		private void RaiseDead() {
			if (Dead != null) {
				Dead(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
