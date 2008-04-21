using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class ZRKChannelInfo {
		private bool readyToFire = false;
		public bool ReadyToFire { get { return readyToFire; } }

		private TimeSpan timeToReload = SimpleZRK.ChannelReloadTime;
		public TimeSpan TimeToReload {
			get { return timeToReload; }
			set { timeToReload = value; }
		}

		internal void Fire() {
			readyToFire = false;
			timeToReload = SimpleZRK.ChannelReloadTime;
		}

		internal void Load() {
			readyToFire = true;
			timeToReload = TimeSpan.Zero;
		}
	}
}
