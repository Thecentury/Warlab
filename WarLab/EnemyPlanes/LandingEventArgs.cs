using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace EnemyPlanes {
	public sealed class LandingEventArgs : EventArgs {
		private readonly ReturnToBaseAim landingAim = ReturnToBaseAim.ReloadOrRefuel;
		public ReturnToBaseAim LandingAim {
			get { return landingAim; }
		} 

		public LandingEventArgs(ReturnToBaseAim landingAim) {
			this.landingAim = landingAim;
		}
	}
}
