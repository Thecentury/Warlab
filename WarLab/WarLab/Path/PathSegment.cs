using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Path {
	public abstract class PathSegment {
		public abstract double Length { get; }
		public abstract Position GetPosition(double progress);
		public abstract Position Start { get; }
		public abstract Position End { get; }
	}
}
