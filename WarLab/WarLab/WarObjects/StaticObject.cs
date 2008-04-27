using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace WarLab {
	/// <summary>
	/// Неподвижный объект мира, видимый на игровой карте.
	/// </summary>
	public abstract class StaticObject : WarObject, IBombDamageable {
		
		#region IDamageable Members

		private double health = 2;
		public double Health {
			get { return health; }
			set { health = value; }
		}

		private void RaiseDead() {
			if (Destroyed != null) {
				Destroyed(this, EventArgs.Empty);
			}
		}
		public event EventHandler Destroyed;

		#endregion

		#region IBombDamageable Members

		void IBombDamageable.MakeDamage(double damage) {
			Verify.IsNonNegative(damage);

			health -= damage;

			Debug.WriteLine(String.Format("{2}: урон {0:F1}, осталось {1:F1}", damage, health, this));
			if (health <= 0) {
				RaiseDead();
			}
		}

		#endregion
	}
}
