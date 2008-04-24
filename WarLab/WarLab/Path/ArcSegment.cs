using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualListener;

namespace WarLab.Path {
	public class ArcSegment : PathSegment {
		private double height;
		public ArcSegment(Vector3D center, double radius, CircleOrientation orientation, Vector3D currentPosition, double angleDelta) {
			this.center = center;
			this.radius = radius;
			this.circleOrientation = orientation;

			height = currentPosition.H;

			Vector3D toCurrentPosition = currentPosition - center;
			double currentAngle = MathHelper.AngleToDegrees(toCurrentPosition.Projection2D.AngleInRad_ZeroOnRight);
			this.startAngle = currentAngle;
			this.endAngle = currentAngle + angleDelta;
			length = (endAngle - startAngle) / 180 * Math.PI * radius;
		}

		private double length;
		private Vector3D center;
		private double radius;
		private CircleOrientation circleOrientation;
		private double startAngle;
		private double endAngle;

		public override double Length {
			get { return length; }
		}

		public override Position GetPosition(double progress) {
			double angle = (1 - progress) * startAngle + progress * endAngle;
			double radians = MathHelper.AngleToRadians(angle);

			double x = center.X + radius * Math.Cos(radians);
			double y = center.Y + radius * Math.Sin(radians);

			Vector3D pos = new Vector3D(x, y, height);
			return new Position(pos, new Vector3D(1, 0));
		}

		public override Position Start {
			get { throw new NotImplementedException(); }
		}

		public override Position End {
			get { return GetPosition(1); }
		}
	}
}
