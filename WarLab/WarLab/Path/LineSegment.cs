using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Path {
	public class LineSegment : PathSegment {
		public LineSegment() { }
		public LineSegment(Vector3D startPoint, Vector3D endPoint) {
			this.startPoint = startPoint;
			this.endPoint = endPoint;
		}

		public override double Length {
			get { return (endPoint - startPoint).Length; }
		}

		public override Position GetPosition(double progress) {
			Vector3D pos = startPoint * (1 - progress) + endPoint * progress;
			return new Position(pos, Direction);
		}

		private Vector3D startPoint;
		public Vector3D StartPoint {
			get { return startPoint; }
			set { startPoint = value; }
		}

		private Vector3D endPoint;
		public Vector3D EndPoint {
			get { return endPoint; }
			set { endPoint = value; }
		}

		public Vector3D Direction {
			get { return (endPoint - startPoint).Normalize(); }
		}

		public override Position Start {
			get { return new Position(startPoint, Direction); }
		}

		public override Position End {
			get { return new Position(endPoint, Direction); }
		}
	}
}
