using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.WarObjects;

namespace EnemyPlanes {
	public class StaticTarget : OurStaticObject {
		#region ISpriteSource Members

		public Vector3D Orientation {
			get { return new Vector3D(1.0, 0.0, 0.0); }
		}

		#endregion
	}
}
