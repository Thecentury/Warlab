using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab.SampleUI.WarObjects;

namespace WarLab.SampleUI.AI {
	class SamplePlaneAI : DynamicObjectAI {
		Vector3D targetPos = new Vector3D(1000, 1000, 1);

		public override void Update(WarTime warTime) {
			double distance = MathHelper.Distance(ControlledDynamicObject.Position, targetPos);

			double possibility = StaticRandom.NextDouble() * StaticRandom.NextDouble();
			if (possibility > 0.9 || distance < 50) {
				UpdateTargetPos();
			}

			MoveInDirectionOf(targetPos);
		}

		private void UpdateTargetPos() {
			double x = StaticRandom.NextDouble() * 1000;
			double y = StaticRandom.NextDouble() * 1000;
			targetPos = new Vector3D(x, y, 1);
		}
	}
}
