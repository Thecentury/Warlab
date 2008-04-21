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

		public const int NumOfChannels = 5;

		private int numOfEquipment = 10;
		/// <summary>
		/// Количество зарядов.
		/// </summary>
		public int NumOfEquipment {
			get { return numOfEquipment; }
			set {
				Verify.IsNonNegative(value);

				numOfEquipment = value;
			}
		}

		private ZRKChannelInfo[] channels = new ZRKChannelInfo[NumOfChannels];
		public ZRKChannelInfo[] Channels {
			get { return channels; }
		}

		private List<RLSTrajectory> trajectories = new List<RLSTrajectory>();

		protected override void UpdateImpl(WarTime warTime) {
			TimeSpan elapsedSeconds = warTime.ElapsedTime;
			// нечем стрелять
			if (numOfEquipment <= 0)
				return;

			int equipment = numOfEquipment;
			// продолжаем перезаряжать незаряженные каналы
			foreach (var channel in channels.Where(ch => !ch.ReadyToFire)) {
				if (equipment > 0) {
					equipment--;
					channel.TimeToReload -= elapsedSeconds;
				}
				if (channel.TimeToReload <= TimeSpan.FromSeconds(0)) {
					if (numOfEquipment > 0) {
						//numOfEquipment--;
						channel.Load();
					}
				}
			}

			// todo это грязный хак
			trajectories = new List<RLSTrajectory>((World.SelectSingle<RLS>().AI as RLSAI).AllTrajectories);

			// обработка траекторий
			foreach (var target in trajectories) {
				ZRKChannelInfo channel = channels.FirstOrDefault(ch => ch.ReadyToFire);
				// нашелся свободный канал
				if (channel != null) {

					Vector3D targetPosition = target.InterpolatedPosition(warTime.TotalTime);

					double distance = (targetPosition - Position).Length;
					TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / RocketSpeed);
					TimeSpan explosionTime = warTime.TotalTime + durationOfFlight;
					Vector3D interpolatedTargetPos = target.InterpolatedPosition(explosionTime);

					Vector3D rocketDir = GetRocketDirection(interpolatedTargetPos);
					double targetSpeedProj = rocketDir & target.Direction * target.Speed;

					// выпускаем ракету, только если она сможет догнать цель.
					if (targetSpeedProj < rocketSpeed) {
						channel.Fire();
						numOfEquipment--;
						Debug.WriteLine("ЗРК: Firing missile");
						LaunchRocket(warTime.TotalTime, interpolatedTargetPos);
					}
				}
			}
		}

		private Vector3D GetRocketDirection(Vector3D targetPos) {
			return (targetPos - Position).Normalize();
		}

		private double rocketSpeed = Speed.FromKilometresPerHour(300);
		public double RocketSpeed {
			get { return rocketSpeed; }
			set {
				Verify.IsPositive(value);

				rocketSpeed = value;
			}
		}
		private void LaunchRocket(TimeSpan globalTime, Vector3D targetPosition) {

			double distance = (targetPosition - Position).Length;
			TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / rocketSpeed);

			Debug.WriteLine("Rocket: " + durationOfFlight.TotalSeconds + " to fly");

			TimeSpan timeToExplode = durationOfFlight + globalTime;

			Rocket rocket = new Rocket
			{
				Speed = rocketSpeed,
				TimeOfExposion = timeToExplode,
				TargetPoint = targetPosition
			};

			World.AddWarObject(rocket, Position);
		}
	}
}
