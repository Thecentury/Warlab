using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using VisualListener;
using System.Diagnostics;

namespace WarLab.WarObjects {
	public abstract class ZRKBase : OurStaticObject {
		private void InitChannels() {
			channels = new ZRKChannelInfo[numOfChannels];
			for (int i = 0; i < numOfChannels; i++) {
				channels[i] = new ZRKChannelInfo();
			}
		}

		protected ZRKBase() {
			InitChannels();
			Importance = Default.ZRKImportance;
		}

		public bool HasFreeChannels {
			get { return channels.Count(ch => ch.ReadyToFire) != 0; }
		}

		public static readonly TimeSpan ChannelReloadTime = TimeSpan.FromSeconds(60);

		protected int numOfChannels = 5;
		public int NumOfChannels {
			get { return numOfChannels; }
			set {
				Verify.IsNonNegative(value);
				numOfChannels = value;
				InitChannels();
			}
		}

		protected int numOfEquipment = 500;
		/// <summary>
		/// Количество зарядов.
		/// </summary>
		public int NumOfEquipment {
			get { return numOfEquipment; }
			set {
				Verify.IsNonNegative(value);

				PropertyInspector.AddValue("numOfEquipment", value);

				numOfEquipment = value;
			}
		}

		protected ZRKChannelInfo[] channels;
		public ZRKChannelInfo[] Channels {
			get { return channels; }
		}

		protected List<RLSTrajectory> trajectories = new List<RLSTrajectory>();

		public const int RelyableTrajectoryAge = 1;
		protected double coverageRadius = Distance.FromMetres(400);
		public double CoverageRadius {
			get { return coverageRadius; }
			set { coverageRadius = value; }
		}

		public bool IsInCoverage(WarObject obj) {
			return IsInCoverage(obj.Position);
		}

		public bool IsInCoverage(Vector3D pos) {
			return pos.Distance2D(Position) <= coverageRadius;
		}

		protected Vector3D GetRocketDirection(Vector3D targetPos) {
			return (targetPos - Position).Normalize();
		}

		protected double rocketSpeed = Speed.FromKilometresPerHour(300);
		public double RocketSpeed {
			get { return rocketSpeed; }
			set {
				Verify.IsPositive(value);

				rocketSpeed = value;
			}
		}

		protected double targetingErrorDistance = Distance.FromMetres(8);
		public double TargetingErrorDistance {
			get { return targetingErrorDistance; }
			set { targetingErrorDistance = value; }
		}

		protected void LaunchRocket(TimeSpan globalTime, Vector3D targetPosition) {

			double distance = (targetPosition - Position).Length;
			TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / rocketSpeed);

			Debug.WriteLine("Rocket: " + durationOfFlight.TotalSeconds + " s to fly");

			TimeSpan timeToExplode = durationOfFlight + globalTime;

			Vector3D targetPositionError = Vector3D.RandomVectorNormalized(targetingErrorDistance) * targetingErrorDistance;

			PropertyInspector.AddValue("Target error", targetPositionError.Length);

			Rocket rocket = new Rocket
			{
				Speed = rocketSpeed,
				TimeOfExposion = timeToExplode,
				TargetPosition = targetPosition + targetPositionError,
				Host = RocketHost.ZRK
			};

			World.AddObject(rocket, Position);
		}

	}
}
