using System;
using System.Collections.Generic;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Неподвижный объект мира, видимый на игровой картею
	/// </summary>
	public abstract class StaticObject : WarObject, IBombDamageable {
		
		#region IBombDamageable Members

		public void MakeDamage(double damage) {
			throw new NotImplementedException();
		}

		#endregion

		#region IDamageable Members

		private double health;
		public double Health {
			get { throw new NotImplementedException(); }
			internal set { health = value; }
		}

		private void RaiseDead() {
			if (Dead != null) {
				Dead(this, EventArgs.Empty);
			}
		}
		public event EventHandler Dead;

		#endregion
	}
}
