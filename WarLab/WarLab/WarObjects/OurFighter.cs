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

		protected override string NameCore {
			get {
				return "Истребитель обороны";
			}
		}
	}
}
