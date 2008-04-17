using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.Test {
	class Program {
		static void Main(string[] args) {
			Vector3D v = new Vector3D(1.2345678, 0.999999, -7.4333333);
			string str = v.ToString();

			Vector2D v1 = new Vector2D(0, 0);
			double azimuth = v1.ToAzimuth();

			double az_90 = new Vector2D(1, 0).ToAzimuth();
			double az_180 = new Vector2D(0, -1).ToAzimuth();
			double az_270 = new Vector2D(-1, 0).ToAzimuth();
			double az_359 = new Vector2D(-0.01, 0.99).Normalize().ToAzimuth();

			Vector2D v2 = Vector2D.FromAzimuth(90);

			Orientation N = new Vector2D(0, 1).ToOrientation();
			Orientation E = new Vector2D(1, 0).ToOrientation();
			Orientation S = new Vector2D(0, -1).ToOrientation();
			Orientation W = new Vector2D(-1, 0).ToOrientation();
		}
	}
}
