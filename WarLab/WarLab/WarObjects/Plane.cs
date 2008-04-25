using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using VisualListener;
using WarLab.WarObjects;

namespace WarLab {
	public abstract class Plane : DynamicObject, IRocketDamageable {
		private Airport airport;
		public Airport Airport {
			get { return airport; }
			internal set { airport = value; }
		}

		public Vector3D AirportPosition {
			get { return airport.Position; }
		}

		protected sealed override void UpdateImpl(WarTime warTime) {
			UpdateCore(warTime);

			Vector3D shift = Orientation * warTime.ElapsedTime.TotalSeconds * Speed;

			FuelLeft -= shift.Length;
			Position += shift;
		}

		/// <summary>
		/// Метод, который будет вызываться каждый такт и именно в котором нужно 
		/// реализовывать ИИ.
		/// </summary>
		/// <param name="warTime">Время.</param>
		protected virtual void UpdateCore(WarTime warTime) { }

		private double fuelLeft = Distance.FromKilometres(10000);
		public double FuelLeft {
			get { return fuelLeft; }
			set {
				Verify.IsFinite(value);

				fuelLeft = value;

				if (fuelLeft < 0.01) {
					RaiseDead();
				}
			}
		}

		/// <summary>
		/// Заправляет самолет - количество топлива увеличивается до MaxFuel.
		/// </summary>
		public void Refuel() {
			FuelLeft = MaxFuel;
		}

		private double maxFuel = Distance.FromKilometres(10000);
		public double MaxFuel {
			get { return maxFuel; }
			set {
				Verify.IsPositive(value);

				maxFuel = value;
			}
		}

		public void Reload() {
			WeaponsLeft = WeaponsCapacity;
		}

		/// Сколько может нести вооружения (бомб или ракет)
		private int weaponsCapacity = 1;
		//сколько осталось оружия
		private int weaponsLeft = 1;

		/// <summary>
		/// Количество оставшегося вооружения. Когда его 0 - надо лететь на базу
		/// </summary>
		public int WeaponsLeft {
			get { return weaponsLeft; }
			set {
				Verify.IsNonNegative(value);

				weaponsLeft = value;
			}
		}

		/// <summary>
		/// Возвращает количество оружия, которое может нести истребитель
		/// </summary>
		public int WeaponsCapacity {
			get { return weaponsCapacity; }
			set {
				Verify.IsNonNegative(value);

				weaponsCapacity = value;
				weaponsLeft = value;
			}
		}


		#region IRocketDamageable Members

		public bool IsDestroyed {
			get { return health <= 0.01; }
		}

		void IRocketDamageable.MakeDamage(double damage) {
			Verify.IsNonNegative(damage);

			health -= damage;

			Debug.WriteLine(String.Format("Plane: {0:F1} damage taken, {1:F1} health left", damage, health));
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
