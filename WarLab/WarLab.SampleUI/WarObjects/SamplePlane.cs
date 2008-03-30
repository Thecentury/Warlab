using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.SampleUI.WarObjects {
	public class SamplePlane : Plane {
		protected override void UpdateCore(WarTime warTime) {
			if (50 <= Position.X && Position.X < 750) {
				MoveInDirectionOf(1000, 1000, 1);
			}
			else {
				MoveInDirectionOf(1000, 0, 1);
			}
		}
	}
}
