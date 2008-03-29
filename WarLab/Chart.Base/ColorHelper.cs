using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ScientificStudio.Charting {
	public static class ColorHelper {
		private readonly static Random random = new Random();

		/// <summary>
		/// Creates color from HSB color space with random hue and saturation and brighness equal to 1.
		/// </summary>
		/// <returns></returns>
		public static Color RandomColor() {
			double hue = random.NextDouble() * 360;
			HSBColor hsbColor = new HSBColor(hue, 1, 1);
			return hsbColor.ToARGB();
		}

		/// <summary>
		/// Creates color with fully random hue and slightly random saturation and brightness.
		/// </summary>
		/// <returns></returns>
		public static Color RandomColorEx() {
			double h = random.NextDouble() * 360;
			double s = random.NextDouble() * 0.5 + 0.5;
			double b = random.NextDouble() * 0.25 + 0.75;
			return new HSBColor(h, s, b).ToARGB();
		}
	}
}
