using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;

namespace WarLab.WarObjects {
	public sealed class SimpleZRK : StaticObject {
		/// <summary>
		/// Создает ЗРК с числом каналов по-умолчанию.
		/// </summary>
		public SimpleZRK() {
			InitChannels();
		}

		private void InitChannels() {
			channels = new ZRKChannelInfo[NumOfChannels];
			for (int i = 0; i < NumOfChannels; i++) {
				channels[i] = new ZRKChannelInfo();
			}
		}

		/// <summary>
		/// Создает ЗРК с заданным числом каналов.
		/// </summary>
		/// <param name="numOfChannels">Число каналов.</param>
		public SimpleZRK(int numOfChannels) {
			Verify.IsNonNegative(numOfChannels);

			NumOfChannels = numOfChannels;

			InitChannels();
		}

		public bool HasFreeChannels {
			get { return channels.Count(ch => ch.ReadyToFire) != 0; }
		}

		public static readonly TimeSpan ChannelReloadTime = TimeSpan.FromSeconds(4);

		public readonly int NumOfChannels = 5;

		private int numOfEquipment = 500;
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

		private ZRKChannelInfo[] channels;
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
			var unloadedChannels = channels.Where(ch => !ch.ReadyToFire).ToList();

			unloadedChannels.Sort((c1, c2) => c1.TimeToReload.CompareTo(c2.TimeToReload));

			foreach (var channel in unloadedChannels) {
				if (equipment > 0) {
					equipment--;
					channel.TimeToReload -= elapsedSeconds;

					// канал заряжен
					if (channel.TimeToReload <= TimeSpan.FromSeconds(0)) {
						if (numOfEquipment > 0) {
							numOfEquipment--;
							channel.Load();
						}
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

						Debug.WriteLine(new Run
						{
							Text = "ЗРК: Firing missile",
							Foreground = Brushes.Blue,
							FontFamily = new FontFamily("Calibri"),
							FontSize = 14
						});
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

			Debug.WriteLine("Rocket: " + durationOfFlight.TotalSeconds + " s to fly");

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
