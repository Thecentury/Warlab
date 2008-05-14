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
	public sealed class ZRK : ZRKBase {
		/// <summary>
		/// Создает ЗРК с числом каналов по-умолчанию.
		/// </summary>
		public ZRK()
			: base() {
		}

		protected override string NameCore {
			get { return "ЗРК"; }
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

	}
}
