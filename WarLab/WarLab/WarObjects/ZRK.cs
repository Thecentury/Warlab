﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;
using VisualListener;

namespace WarLab.WarObjects {
	public sealed class ZRK : OurStaticObject {
		/// <summary>
		/// Создает ЗРК с числом каналов по-умолчанию.
		/// </summary>
		public ZRK() {
			InitChannels();
			Importance = Default.ZRKImportance;
		}

		protected override string NameCore {
			get { return "ЗРК"; }
		}

		private void InitChannels() {
			channels = new ZRKChannelInfo[numOfChannels];
			for (int i = 0; i < numOfChannels; i++) {
				channels[i] = new ZRKChannelInfo();
			}
		}

		public bool HasFreeChannels {
			get { return channels.Count(ch => ch.ReadyToFire) != 0; }
		}

		public static readonly TimeSpan ChannelReloadTime = TimeSpan.FromSeconds(10);

		private int numOfChannels = 5;
		public int NumOfChannels {
			get { return numOfChannels; }
			set {
				Verify.IsNonNegative(value);
				numOfChannels = value;
				InitChannels();
			}
		}

		private int numOfEquipment = 500;
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

		private ZRKChannelInfo[] channels;
		public ZRKChannelInfo[] Channels {
			get { return channels; }
		}

		private List<RLSTrajectory> trajectories = new List<RLSTrajectory>();

		public const int RelyableTrajectoryAge = 1;
		private double coverageRadius = Distance.FromMetres(400);
		public double CoverageRadius {
			get { return coverageRadius; }
			set { coverageRadius = value; }
		}

		public bool IsInCoverage(WarObject obj) {
			return obj.Position.Distance2D(Position) <= coverageRadius;
		}

		protected override void UpdateImpl(WarTime warTime) {
			TimeSpan elapsedSeconds = warTime.ElapsedTime;
			// нечем стрелять
			if (numOfEquipment <= 0)
				return;

			// продолжаем перезаряжать незаряженные каналы
			var unloadedChannels = channels.Where(ch => !ch.ReadyToFire).ToList();

			unloadedChannels.Sort((c1, c2) => c1.TimeToReload.CompareTo(c2.TimeToReload));

			int equipment = numOfEquipment;
			foreach (var channel in unloadedChannels) {
				if (equipment > 0) {
					equipment--;
					channel.TimeToReload -= elapsedSeconds;

					// канал заряжен
					if (channel.TimeToReload <= TimeSpan.FromSeconds(0.001)) {
						if (numOfEquipment > 0) {
							NumOfEquipment--;
							channel.Load();
						}
					}
				}
			}

			var rlses = World.SelectAll<RLS>();

			// todo это грязный хак
			trajectories = rlses.Select(rls => (RLSAI)rls.AI).SelectMany(rlsai => rlsai.AllTrajectories).
				Where(t => t.NumOfSteps >= RelyableTrajectoryAge).ToList();

			trajectories.Sort((t1, t2) =>
				t1.Position.DistanceTo(Position).CompareTo(t2.Position.DistanceTo(Position)));

			// обработка траекторий
			foreach (var traject in trajectories) {
				ZRKChannelInfo channel = channels.FirstOrDefault(ch => ch.ReadyToFire);

				// нашелся свободный канал
				if (channel != null) {

					Vector3D targetPosition = traject.ExtrapolatedPosition(warTime.TotalTime);

					double distance = (targetPosition - Position).Length;
					TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / RocketSpeed);
					TimeSpan explosionTime = warTime.TotalTime + durationOfFlight;
					Vector3D interpolatedTargetPos = traject.ExtrapolatedPosition(explosionTime);

					Vector3D rocketDir = GetRocketDirection(interpolatedTargetPos);
					double targetSpeedProj = rocketDir & traject.Direction * traject.Speed;

					// выпускаем ракету, только если она сможет догнать цель.
					if (targetSpeedProj < rocketSpeed && interpolatedTargetPos.Distance2D(Position) < CoverageRadius) {
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

		private double targetingErrorDistance = Distance.FromMetres(8);
		public double TargetingErrorDistance {
			get { return targetingErrorDistance; }
			set { targetingErrorDistance = value; }
		}

		private void LaunchRocket(TimeSpan globalTime, Vector3D targetPosition) {

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
