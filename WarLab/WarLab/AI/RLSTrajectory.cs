using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using WarLab.WarObjects;
using System.ComponentModel;

namespace WarLab.AI {
	[TypeConverter(typeof(PropertiesVisibleTypeConverter))]
	public class RLSTrajectory {
		public RLSTrajectory(Vector3D position, int createdRLSTurn, TimeSpan createdTime, EnemyPlane plane) {
			Position = position;
			HasDirection = false;
			NumOfSteps = 1;
			LastUpdateTurn = createdRLSTurn;
			LastUpdateTime = createdTime;
			Plane = plane;
		}

		public void Update(Vector3D newPosition, TimeSpan updateTime, int rlsTurn) {
			HasDirection = true;
			Vector3D shift = newPosition - Position;

			TimeSpan elapsedTime = updateTime - LastUpdateTime;
			if (elapsedTime.TotalSeconds > 0) {
				Speed = shift.Length / elapsedTime.TotalSeconds;
			}
			LastUpdateTime = updateTime;

			Direction = shift.Normalize();
			Position = newPosition;

			NumOfSteps++;
			LastUpdateTurn = rlsTurn;
		}

		public Vector3D Position { get; private set; }
		public Vector3D Direction { get; private set; }
		public double Speed { get; private set; }
		public int NumOfSteps { get; private set; }
		public int LastUpdateTurn { get; private set; }
		public bool HasDirection { get; private set; }
		public TimeSpan LastUpdateTime { get; private set; }
		public EnemyPlane Plane { get; private set; }
		public bool IsDestroyed { get { return Plane.IsDestroyed; } }

		private ZRKBase assignedZRK = null;
		public ZRKBase AssignedZRK {
			get { return assignedZRK; }
			set { assignedZRK = value; }
		}

		public RLSTrajectory Clone() {
			RLSTrajectory t = (RLSTrajectory)MemberwiseClone();

			return t;
		}

		public bool IsCloseTo(RLSTrajectory otherTraj, double errorStrobe) {
			return Position.DistanceTo(otherTraj.Position) < errorStrobe &&
				(Direction & otherTraj.Direction) > minScalarProj;
		}

		public static readonly double MaxPlaneSpeed = WarLab.Speed.FromKilometresPerHour(300);

		/// <summary>
		/// Предполагаемое положение цели в момент времени <paramref name="totalTime"/>.
		/// </summary>
		/// <param name="totalTime"></param>
		/// <returns></returns>
		public Vector3D ExtrapolatedPosition(TimeSpan totalTime) {
			TimeSpan delta = totalTime - LastUpdateTime;
			return Position + Speed * delta.TotalSeconds * Direction;
		}

		const double minScalarProj = 0.2;
		public bool IsInStrobe(Vector3D point, TimeSpan totalTime, double errorDistance) {
			double deltaSeconds = (totalTime - LastUpdateTime).TotalSeconds;
			if (HasDirection) {
				double distance = Speed * deltaSeconds;
				Vector3D newPos = Position + distance * Direction;
				double realDist = MathHelper.Distance(point, newPos);
				bool res = (realDist <= errorDistance) && ((Direction & (point - Position).Normalize()) > minScalarProj);
				return res;
			}
			else {
				double expectedDist = MaxPlaneSpeed * deltaSeconds;
				double realDist = MathHelper.Distance(point, Position);
				bool res = realDist <= expectedDist;
				if (!res) { }
				return res;
			}
		}
	}
}
