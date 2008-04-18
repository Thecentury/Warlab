#define full

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;
using System.Diagnostics;

namespace WarLab.AI {
	[Controls(typeof(RLS))]
	public sealed class RLSAI : WarAI {

		private readonly double errorDistance = Distance.FromMetres(0);
		private readonly double strobeError = Distance.FromMetres(0); // 100

		List<RLSTrajectory> trajectories = new List<RLSTrajectory>();
		private RLS RLS {
			get { return ControlledObject as RLS; }
		}

		public IEnumerable<RLSTrajectory> AllTrajectories {
			get { return trajectories; }
		}

		private int minNumOfStepsInPreciseTrajectory = 5;
		public IEnumerable<RLSTrajectory> OldTrajectories {
			get { return trajectories.Where(t => t.NumOfSteps != minNumOfStepsInPreciseTrajectory); }
		}

		public IEnumerable<RLSTrajectory> NewlyCreatedTrajectories {
			get { return trajectories.Where(t => t.NumOfSteps == minNumOfStepsInPreciseTrajectory); }
		}

		NormalDistribution rnd = new NormalDistribution();
		private Vector3D RandomVector() {

			double x = rnd.Generate(0, errorDistance);
			double y = rnd.Generate(0, errorDistance);
			double z = rnd.Generate(0, errorDistance);
			return new Vector3D(x, y, z).Normalize();
		}

		private int turnNum = 0;
		private TimeSpan fromPrevTurn = new TimeSpan();
		public override void Update(WarTime time) {
			fromPrevTurn += time.ElapsedTime;
			trajectories.RemoveAll(tr => tr.IsDestroyed);

#if full
			if (fromPrevTurn > RLS.RotationPeriod) {
#endif
#if !full
				var enemyPlanes = RLS.GetPlanesInSector();
#else
				var enemyPlanes = RLS.GetPlanesInCoverage();
#endif

				var examinedPlanes = new List<EnemyPlane>();
				var newTrajectories = new List<RLSTrajectory>();
				var trajectsToRemove = new List<RLSTrajectory>();

				foreach (var traj in trajectories) {
					var appropriatePlanes = enemyPlanes.
						Where(plane => traj.IsInStrobe(plane.Position, fromPrevTurn, strobeError));

					bool wereContinuations = false;
					foreach (var plane in appropriatePlanes) {
						if (!examinedPlanes.Contains(plane)) {
							examinedPlanes.Add(plane);
						}

						wereContinuations = true;

						RLSTrajectory newTraj = traj.Clone();

						Vector3D newPos = plane.Position + errorDistance * RandomVector() / newTraj.NumOfSteps;

						newTraj.Update(newPos, fromPrevTurn, turnNum);
						newTrajectories.Add(newTraj);
					}

					if (wereContinuations) {
						trajectsToRemove.Add(traj);
					}
				}

				foreach (var traj in trajectsToRemove) {
					trajectories.Remove(traj);
				}

				var unexaminedPlanes = enemyPlanes.Where(plane => !examinedPlanes.Contains(plane));
				foreach (var plane in unexaminedPlanes) {
					trajectories.Add(new RLSTrajectory(
						plane.Position + errorDistance * RandomVector(),
						turnNum,
						time.TotalTime,
						plane));
				}

				trajectories.AddRange(newTrajectories);

				trajectories.RemoveAll(traj => (turnNum - traj.LastUpdateTurn) >= 3);

				// удаляем все не поместившиеся траектории после заполнения каналов
				if (trajectories.Count > RLS.ChannelsNum) {
					trajectories.RemoveRange(RLS.ChannelsNum, trajectories.Count - RLS.ChannelsNum);
				}

				int i = 0;
				while (i < trajectories.Count) {
					int j = i + 1;
					while (j < trajectories.Count) {
						if (trajectories[i].IsCloseTo(trajectories[j], errorDistance)) {
							// todo возможно, проводить тут интерполяцию
							trajectories.RemoveAt(j);
						}
						else {
							j++;
						}
					}
					i++;
				}

#if !full
			if (fromPrevTurn > RLS.RotationPeriod) {
#endif
				fromPrevTurn = new TimeSpan();
				turnNum++;
#if !full
			}
#else
			}
#endif

		}
	}
}
