using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace WarLab.WarObjects {
	public sealed class ZRK : OurStaticObject {
		public ZRK() {
			for (int i = 0; i < NumOfChannels; i++) {
				channels[i] = new ZRKChannelInfo();
			}
		}

		public bool HasFreeChannels {
			get { return channels.Count(ch => ch.ReadyToFire) != 0; }
		}

		static readonly TimeSpan reloadTime = TimeSpan.FromSeconds(60);

		public const int NumOfChannels = 10;

		private int numOfEquipment = 50;
		public int NumOfEquipment {
			get { return numOfEquipment; }
			set { numOfEquipment = value; }
		}

		private ZRKChannelInfo[] channels = new ZRKChannelInfo[NumOfChannels];
		public ZRKChannelInfo[] Channels {
			get { return channels; }
		}

		private readonly List<RLSTrajectory> trajectories = new List<RLSTrajectory>();

		protected override void UpdateImpl(WarTime warTime) {
			// продолжаем перезаряжать незаряженные каналы
			TimeSpan elapsedSeconds = warTime.ElapsedTime;
			foreach (var channel in channels.Where(ch => !ch.ReadyToFire)) {
				channel.TimeToReload -= elapsedSeconds;
				if (channel.TimeToReload <= TimeSpan.FromSeconds(0)) {
					channel.Load();
				}
			}

			foreach (var target in trajectories) {
				ZRKChannelInfo channel = channels.First(ch => ch.ReadyToFire);
				channel.Fire();

				LaunchRocket(warTime.TotalTime, target.Position);
			}
		}

		private void LaunchRocket(TimeSpan globalTime, Vector3D targetPosition) {
			Rocket rocket = new Rocket();

			double distance = (targetPosition - Position).Length;
			TimeSpan timeToExplode = TimeSpan.FromSeconds(distance / rocket.Speed) + globalTime;

			rocket.TimeOfExposion = timeToExplode;

			World.AddWarObject(rocket, Position);
		}
	}
}
