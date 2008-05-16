using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using System.Diagnostics;
using System.Windows.Documents;
using System.Windows.Media;
using VisualListener;

namespace WarLab.WarObjects {
	public sealed class SingleChannelZRK : ZRKBase {
		/// <summary>
		/// Создает ЗРК с одним каналом.
		/// </summary>
		public SingleChannelZRK()
			: base() {
			NumOfChannels = 1;
			NumOfEquipment = 5;
		}

		protected override string NameCore {
			get { return "ЗРК"; }
		}


		RLSTrajectory preferredTraj;
		protected override void UpdateImpl(WarTime time) {
			TimeSpan elapsedSeconds = time.ElapsedTime;
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

			if (preferredTraj != null && (preferredTraj.Plane.IsDestroyed || preferredTraj.Plane.IsLanded)) {
				preferredTraj = null;
			}

			if (preferredTraj != null && IsInCoverage(preferredTraj.ExtrapolatedPosition(time.TotalTime))) {
				ProcessTrajectory(time, preferredTraj);
			}
			else {
				var rlses = World.SelectAll<RLS>();

				// todo это грязный хак
				var trajectories = rlses.Select(rls => (RLSAI)rls.AI).SelectMany(rlsai => rlsai.AllTrajectories).
					Where(t => t.NumOfSteps >= RelyableTrajectoryAge && !IsAssignedToThis(t)).ToList();

				trajectories.Sort((t1, t2) =>
					t1.Position.DistanceTo(Position).CompareTo(t2.Position.DistanceTo(Position)));

				// обработка траекторий
				foreach (var traject in trajectories) {
					ProcessTrajectory(time, traject);
				}
			}
		}

		private void ProcessTrajectory(WarTime time, RLSTrajectory traject) {
			ZRKChannelInfo channel = channels.FirstOrDefault(ch => ch.ReadyToFire);

			// нашелся свободный канал
			if (channel != null) {

				Vector3D targetPosition = traject.ExtrapolatedPosition(time.TotalTime);

				double distance = (targetPosition - Position).Length;
				TimeSpan durationOfFlight = TimeSpan.FromSeconds(distance / RocketSpeed);
				TimeSpan explosionTime = time.TotalTime + durationOfFlight;
				Vector3D extrapolatedTargetPos = traject.ExtrapolatedPosition(explosionTime);

				Vector3D rocketDir = GetRocketDirection(extrapolatedTargetPos);
				double targetSpeedProj = rocketDir & traject.Direction * traject.Speed;

				// выпускаем ракету, только если она сможет догнать цель.
				if (targetSpeedProj < rocketSpeed && extrapolatedTargetPos.Distance2D(Position) < CoverageRadius) {
					traject.AssignedZRK = this;
					channel.Fire();
					preferredTraj = traject;

					Debug.WriteLine(new Run
					{
						Text = "ЗРК: Firing missile",
						Foreground = Brushes.Blue,
						FontFamily = new FontFamily("Calibri"),
						FontSize = 14
					});
					LaunchRocket(time.TotalTime, extrapolatedTargetPos);
				}
				else {
					traject.AssignedZRK = null;
				}
			}
		}
	}
}
