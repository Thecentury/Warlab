using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

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

		private double fuelLeft = Distance.FromKilometres(10000);
		public double FuelLeft {
			get { return fuelLeft; }
			set {
				fuelLeft = value;
				if (fuelLeft < 0.01) {
					RaiseDead();
				}
			}
		}

		#region IRocketDamageable Members

		public bool IsDestroyed {
			get { return health <= 0.01; }
		}

		void IRocketDamageable.MakeDamage(double damage) {
			Verify.DoubleIsPositive(damage);

			health -= damage;
	
			Debug.WriteLine(String.Format("Plane: {0} damage taken, {1} health left", damage, health));
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

		public event EventHandler Destroyed;
		private void RaiseDead() {
			if (Destroyed != null) {
				Destroyed(this, EventArgs.Empty);
			}
		}

		#endregion
	}
}
