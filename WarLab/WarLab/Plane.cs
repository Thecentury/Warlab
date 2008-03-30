using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	public abstract class Plane : DynamicObject, IRocketDamageable {
		WarTime time;
		public sealed override void Update(WarTime warTime) {
			time = warTime;
			moved = false;
			UpdateCore(warTime);
		}

		/// <summary>
		/// Метод, который будет вызываться каждый такт и именно в котором нужно реализовывать ИИ.
		/// </summary>
		/// <param name="warTime">The war time.</param>
		protected abstract void UpdateCore(WarTime warTime);

		bool moved = false;
		/// <summary>
		/// Перемещает самолет в направлении точки.
		/// </summary>
		/// <param name="moveTo">Точка, в направлении которой происходит перемещение.</param>
		/// <remarks>Может быть вызван только один раз за такт.</remarks>
		protected void MoveInDirectionOf(Vector3D moveTo) {
			if (!moved) {
				Vector3D dir = (moveTo - Position).Normalize();
				Orientation = dir.Projection2D;
				Vector3D shift = dir * time.ElapsedTime.TotalSeconds * Speed;
				FuelLeft -= shift.Length;
				Position += shift;
				moved = true;
			}
			else {
				throw new InvalidOperationException("Нельзя вызывать этот метод более одного раза за такт");
			}
		}

		protected void MoveInDirectionOf(double x, double y, double h) {
			MoveInDirectionOf(new Vector3D(x, y, h));
		}

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
			Verify.Double(damage);

			health -= damage;
			if (health <= 0.01) {
				RaiseDead();
			}
		}

		private static void Double(double damage) {
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
