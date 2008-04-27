using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class OurFighter : Plane {
		public OurFighter() {
			WeaponsCapacity = Default.OurFighterWeapons;
			Speed = Default.FighterSpeed;
		}
	}
}
