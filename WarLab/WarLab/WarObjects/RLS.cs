using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class RLS : StaticObject {
		protected override void UpdateImpl(WarTime warTime) {
			base.UpdateImpl(warTime);
		}

		private double coverageRadius = Distance.FromMetres(1000);
		public double CoverageRadius {
			get { return coverageRadius; }
			set { coverageRadius = value; }
		}

		private double errorPossibility = 0.05;

		private int channelsNum = 100;
		public int ChannelsNum {
			get { return channelsNum; }
			set { channelsNum = value; }
		}

		private bool IsCaught(Vector3D targetPos) {
			double dist = (targetPos - Position).Length;
			double chance = StaticRandom.NextDouble();
			bool res = chance < (1 - dist / coverageRadius * errorPossibility);
			if (!res) { }
			return res;
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
