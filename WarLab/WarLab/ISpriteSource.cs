using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public interface ISpriteSource {
		Vector3D Orientation { get; }
		Vector3D Position { get; }
	}
}
