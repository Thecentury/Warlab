using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WarLab.WarObjects {
	[TypeConverter(typeof(ZRKChannelInfoConverter))]
	public sealed class ZRKChannelInfo {
		private bool readyToFire = false;
		public bool ReadyToFire { get { return readyToFire; } }

		private TimeSpan timeToReload = ZRK.ChannelReloadTime;
		public TimeSpan TimeToReload {
			get { return timeToReload; }
			internal set { timeToReload = value; }
		}

		internal void Fire() {
			readyToFire = false;
			timeToReload = ZRK.ChannelReloadTime;
		}

		internal void Load() {
			readyToFire = true;
			timeToReload = TimeSpan.Zero;
		}
	}
}
