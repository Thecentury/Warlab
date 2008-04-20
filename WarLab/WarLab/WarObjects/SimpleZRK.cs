using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using System.Diagnostics;

namespace WarLab.WarObjects {
	public sealed class SimpleZRK : StaticObject {
		public SimpleZRK() {
			for (int i = 0; i < NumOfChannels; i++) {
				channels[i] = new ZRKChannelInfo();
			}
		}

		public bool HasFreeChannels {
			get { return channels.Count(ch => ch.ReadyToFire) != 0; }
		}

		public static readonly TimeSpan ChannelReloadTime = TimeSpan.FromSeconds(4);

		public const int NumOfChannels = 1;

		private int numOfEquipment = 50;
		public int NumOfEquipment {
			get { return numOfEquipment; }
			set { numOfEquipment = value; }
		}

		public ZRKChannelInfo[] channels = new ZRKChannelInfo[NumOfChannels];
		private List<RLSTrajectory> trajectories = new List<RLSTrajectory>();

		protected override void UpdateImpl(WarTime warTime) {
			// продолжаем перезаряжать незаряженные каналы
			TimeSpan elapsedSeconds = warTime.ElapsedTime;
			foreach (var channel in channels.Where(ch => !ch.ReadyToFire)) {
				channel.TimeToReload -= elapsedSeconds;
				if (channel.TimeToReload <= TimeSpan.FromSeconds(0)) {
					channel.ReadyToFire = true;
				}
			}

			// todo это грязный хак
			trajectories = new List<RLSTrajectory>((World.SelectSingle<RLS>().AI as RLSAI).AllTrajectories);

			foreach (var target in trajectories) {
				ZRKChannelInfo channel = channels.FirstOrDefault(ch => ch.ReadyToFire);
				if (channel != null) {
					channel.Fire();
					Debug.WriteLine("ZRK: Firing missile");

					Vector3D targetPosition = target.InterpolatedPosition(warTime.TotalTime);

					double distance = (targetPosition - Position).Length;
					TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / rocketSpeed);
					TimeSpan explosionTime = warTime.TotalTime + durationOfFlight;
					Vector3D interpolatedTargetPos = target.InterpolatedPosition(explosionTime);

					LaunchRocket(warTime.TotalTime, interpolatedTargetPos);
				}
			}
		}

		static readonly double rocketSpeed = Speed.FromKilometresPerHour(300);
		private void LaunchRocket(TimeSpan globalTime, Vector3D targetPosition) {
			Rocket rocket = new Rocket();
			rocket.Speed = rocketSpeed;

			double distance = (targetPosition - Position).Length;
			TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / rocket.Speed);
			Debug.WriteLine("Rocket: " + durationOfFlight.TotalSeconds + " to fly");
			TimeSpan timeToExplode = durationOfFlight + globalTime;

			rocket.TimeOfExposion = timeToExplode;
			rocket.TargetPoint = targetPosition;

			World.AddWarObject(rocket, Position);
		}
	}
}
