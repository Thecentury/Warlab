using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab.AI {
	public class RLSTrajectory {
		public RLSTrajectory(Vector3D position, int createdRLSTurn) {
			Position = position;
			hasDirection = false;
			NumOfSteps = 1;
			LastUpdateTurn = createdRLSTurn;
		}

		public void Update(Vector3D newPosition, TimeSpan elapsedTime, int rlsTurn) {
			hasDirection = true;
			Vector3D shift = newPosition - Position;
			Speed = shift.Length / elapsedTime.TotalSeconds;

			Direction = shift.Normalize();
			Position = newPosition;

			NumOfSteps++;
			LastUpdateTurn = rlsTurn;
		}

		public Vector3D Position { get; set; }
		public Vector3D Direction { get; private set; }
		public double Speed { get; private set; }
		public int NumOfSteps { get; private set; }
		public int LastUpdateTurn { get; private set; }
		bool hasDirection = false;

		public bool HasDirection {
			get { return hasDirection; }
		}

		public RLSTrajectory Clone() {
			RLSTrajectory t = (RLSTrajectory)MemberwiseClone();

			return t;
		}

		public bool IsCloseTo(RLSTrajectory otherTraj, double errorStrobe) {
			return Position.DistanceTo(otherTraj.Position) < errorStrobe &&
				(Direction & otherTraj.Direction) > 0.9;
		}

		public static readonly double MaxPlaneSpeed = WarLab.Speed.FromKilometresPerHour(1000);

		public bool IsInStrobe(Vector3D point, TimeSpan elapsedTime, double errorDistance) {
			if (hasDirection) {
				double distance = Speed * elapsedTime.TotalSeconds;
				Vector3D newPos = Position + distance * Direction;
				double realDist = MathHelper.Distance(point, newPos);
				bool res = realDist <= errorDistance;
				if (!res) { }
				return res;
			}
			else {
				double expectedDist = MaxPlaneSpeed * elapsedTime.TotalSeconds;
				double realDist = MathHelper.Distance(point, Position);
				bool res = realDist <= expectedDist;
				if (!res) { }
				return res;
			}
		}
	}
}
