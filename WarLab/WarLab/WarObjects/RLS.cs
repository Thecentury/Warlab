#define full

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab.WarObjects {
	public sealed class RLS : OurStaticObject {

#if !full
		private double radarAngle = 0;
		public double RadarAngle {
			get { return radarAngle; }
		}

		private double prevRadarAngle = 0;
#endif

		protected override void UpdateImpl(WarTime warTime) {
#if !full
			prevRadarAngle = radarAngle;
			radarAngle += RotationSpeed * warTime.ElapsedTime.TotalSeconds;
#endif

			base.UpdateImpl(warTime);
		}

		private double coverageRadius = Distance.FromMetres(1000);
		public double CoverageRadius {
			get { return coverageRadius; }
			set { coverageRadius = value; }
		}

		private TimeSpan rotationPeriod = TimeSpan.FromSeconds(3);
		public TimeSpan RotationPeriod {
			get { return rotationPeriod; }
			set { rotationPeriod = value; }
		}

		/// <summary>
		/// Скорость вращения радара, радиан/сек.
		/// </summary>
		/// <value>The rotation speed.</value>
		public double RotationSpeed {
			get { return 2 * Math.PI / rotationPeriod.TotalSeconds; }
		}

		private double errorPossibility = 0.005;

		private int channelsNum = 100;
		public int ChannelsNum {
			get { return channelsNum; }
			set { channelsNum = value; }
		}

		private bool IsCaught(Vector3D targetPos) {
			double dist = (targetPos - Position).Length;
			double chance = StaticRandom.NextDouble();
			return chance < (1 - dist / coverageRadius * errorPossibility);
		}

#if !full
		public IEnumerable<EnemyPlane> GetPlanesInSector() {
			double startAngle = radarAngle;
			double endAngle = prevRadarAngle;

			return GetPlanesInCoverage()
				.Where(plane => IsPlaneInSector(plane, startAngle, endAngle));
		}
#endif

		private bool IsPlaneInSector(Plane plane, double startAngle, double endAngle) {
			Vector3D toPlane = plane.Position - Position;
			toPlane.H = 0;
			toPlane = toPlane.Normalize();

			Vector3D startVec = new Vector3D(Math.Cos(startAngle), Math.Sin(startAngle));
			Vector3D endVec = new Vector3D(Math.Cos(endAngle), Math.Sin(endAngle));

			Vector3D planeToStart = startVec - toPlane;
			Vector3D planeToEnd = endVec - toPlane;
			return (planeToStart & planeToEnd) < 0;
		}

		public IEnumerable<EnemyPlane> GetPlanesInCoverage() {
			return World.SelectAll<EnemyPlane>().
				Where(plane => IsInCoverage(plane.Position) && IsCaught(plane.Position));
		}

		public bool IsInCoverage(Vector3D point) {
			return MathHelper.Distance(Position, point) <= coverageRadius;
		}
	}
}
