using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class ZRKChannelInfo {
		public bool ReadyToFire = true;
		public TimeSpan TimeToReload = new TimeSpan();

		internal void Fire() {
			ReadyToFire = false;
			TimeToReload = SimpleZRK.ChannelReloadTime;
		}
	}
}
