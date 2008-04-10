using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Path {
	public class ArcSegment : PathSegment {
		public ArcSegment() { }
		public ArcSegment(Vector3D center, double radius, CircleOrientation orientation, double startAngle, double endAngle) {
			this.center = center;
			this.radius = radius;
			this.circleOrientation = orientation;
			this.startAngle = startAngle;
			this.endAngle = endAngle;
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
			double radians = Math.PI / 180 * angle;

			double x = center.X + radius * Math.Cos(radians);
			double y = center.Y + radius * Math.Sin(radians);

			Vector3D pos = new Vector3D(x, y, 0);
			return new Position(pos, new Vector3D());
		}

		public override Position Start {
			get { throw new NotImplementedException(); }
		}

		public override Position End {
			get { throw new NotImplementedException(); }
		}
	}
}
