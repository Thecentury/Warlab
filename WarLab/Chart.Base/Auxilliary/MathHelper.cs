﻿using System;
using System.Windows;

namespace ScientificStudio.Charting.Auxilliary {
	internal static class MathHelper {
		internal static double Clamp(double d, double min, double max) {
			return Math.Max(min, Math.Min(d, max));
		}

		internal static double Clamp_01(double d) {
			return Math.Max(0, Math.Min(d, 1));
		}

		internal static void Clamp_01(ref double d) {
			d = Math.Max(0, Math.Min(d, 1));
		}

		internal static Rect CreateRectByPoints(double xMin, double yMin, double xMax, double yMax) {
			return new Rect(new Point(xMin, yMin), new Point(xMax, yMax));
		}
	}
}
