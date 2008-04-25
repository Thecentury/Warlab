using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyPlanes {
	/// <summary>
	/// Цель посадки
	/// </summary>
	public enum LandingAim {
		ReloadOrRefuel,
		NoTargets
	}

	public sealed class LandingEventArgs : EventArgs {
		private readonly LandingAim landingAim = LandingAim.ReloadOrRefuel;
		public LandingAim LandingAim {
			get { return landingAim; }
		} 

		public LandingEventArgs(LandingAim landingAim) {
			this.landingAim = landingAim;
		}
	}
}
