using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace WarLab.SampleUI {
	public class WorldRunner : DispatcherObject {
		public WorldRunner() {
			this.Dispatcher.Hooks.DispatcherInactive += Hooks_DispatcherInactive;
		}

		private TimeSpan totalTime = new TimeSpan();
		private TimeSpan tickDelta = TimeSpan.FromMilliseconds(30);
		
		private void Hooks_DispatcherInactive(object sender, EventArgs e) {
			totalTime.Add(tickDelta);
			
			WarTime time = new WarTime(tickDelta, totalTime);
			
			World.Instance.Update(time);
		}
	}
}
