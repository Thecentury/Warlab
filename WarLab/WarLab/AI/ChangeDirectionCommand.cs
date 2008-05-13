using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab.AI {
	public class ChangeDirectionCommand : IAICommand {
		private readonly DynamicObject target;
		private readonly Vector3D direction;
		private const double minScalarProj = 0.9995;

		public ChangeDirectionCommand(DynamicObject target, Vector3D direction, bool smooth) {
			if (target == null)
				throw new ArgumentNullException("target");

			Verify.IsInSegment(direction.Length, 0.99, 1.01);

#if !true
			if (smooth && (target.Orientation.Projection2D & direction.Projection2D) < minScalarProj) {
				Vector3D sideVec = target.Orientation.LeftVector();
				double angle = Math.Acos(minScalarProj);

				if ((sideVec & direction.Projection2D) < 0) {
					sideVec *= -1;
					angle *= -1;
				}

				double cos = Math.Cos(angle);
				double sin = Math.Sin(angle);

				double x = target.Orientation.X;
				double y = target.Orientation.Y;

				Vector3D newDirection = new Vector3D(
					cos * x + sin * y,
					-sin * x + cos * y,
					direction.H).Normalize();


				direction = newDirection;
			}
#endif

			this.target = target;
			this.direction = direction;
		}

		public void Execute() {
			target.Orientation = direction;
		}

	}
}
