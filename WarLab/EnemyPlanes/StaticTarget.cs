using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;

namespace EnemyPlanes {
	public class StaticTarget : StaticObject, ISpriteSource {
		protected override void UpdateImpl(WarTime warTime) {
			base.UpdateImpl(warTime);
		}

		#region ISpriteSource Members

		public Vector3D Orientation {
			get { return new Vector3D(1.0, 0.0, 0.0); }
		}

		#endregion
	}
}
