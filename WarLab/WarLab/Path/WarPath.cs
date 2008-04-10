using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Path {
	public sealed class WarPath {
		public WarPath(params PathSegment[] segments) {
			this.segments.AddRange(segments);

			currentSegment = segments[0];
		}

		private readonly List<PathSegment> segments = new List<PathSegment>();

		public bool IsFinished {
			get { return passedDistance > TotalLength; }
		}

		private double currSegmStartDistance = 0;
		private double passedDistance = 0;
		private PathSegment currentSegment;
		private int currSegmIndex = 0;

		public Position GetPosition(double delta) {
			passedDistance += delta;
			if (passedDistance > (currSegmStartDistance + currentSegment.Length)) {
				currSegmIndex++;
				if (currSegmIndex >= segments.Count) {
					return currentSegment.End;
				}
				else {
					currentSegment = segments[currSegmIndex];
					currSegmStartDistance += segments[currSegmIndex - 1].Length;
				}
			}
			double progress = (passedDistance - currSegmStartDistance) / currentSegment.Length;
			return currentSegment.GetPosition(progress);
		}

		public double TotalLength {
			get {
				return segments.Sum(s => s.Length);
			}
		}
	}
}
