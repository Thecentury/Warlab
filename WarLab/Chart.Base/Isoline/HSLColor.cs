using System;
using System.Windows.Media;

namespace ScientificStudio.Charting {
	public struct HSBColor {
		private double hue;
		/// <summary>
		/// Hue; [0, 360]
		/// </summary>
		public double Hue {
			get { return hue; }
			set { hue = value; }
		}

		private double saturation;
		/// <summary>
		/// Saturation; [0, 1]
		/// </summary>
		public double Saturation {
			get { return saturation; }
			set { saturation = value; }
		}

		private double brightness;
		/// <summary>
		/// Brightness; [0, 1]
		/// </summary>
		public double Brightness {
			get { return brightness; }
			set { brightness = value; }
		}

		private double alpha;
		/// <summary>
		/// Alpha; [0, 1]
		/// </summary>
		public double Alpha {
			get { return alpha; }
			set { alpha = value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HSBColor"/> struct.
		/// </summary>
		/// <param name="hue">The hue; [0; 360]</param>
		/// <param name="saturation">The saturation; [0, 1]</param>
		/// <param name="brightness">The brightness; [0, 1]</param>
		public HSBColor(double hue, double saturation, double brightness) {
			this.hue = hue;
			this.saturation = saturation;
			this.brightness = brightness;
			alpha = 1;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HSBColor"/> struct.
		/// </summary>
		/// <param name="hue">The hue; [0, 360]</param>
		/// <param name="saturation">The saturation; [0, 1]</param>
		/// <param name="brightness">The brightness; [0, 1]</param>
		/// <param name="alpha">The alpha; [0, 1]</param>
		public HSBColor(double hue, double saturation, double brightness, double alpha) {
			this.hue = hue;
			this.saturation = saturation;
			this.brightness = brightness;
			this.alpha = alpha;
		}

		private static void ToARGB(double r1, double b1, double g1, out double r, out double g, out double b) {
			r = Math.Floor(r1 * 255 + 0.5);
			g = Math.Floor(g1 * 255 + 0.5);
			b = Math.Floor(b1 * 255 + 0.5);
		}

		public static HSBColor FromARGB(Color color) {
			HSBColor res = new HSBColor();
			double limit = 256;
			res.Alpha = color.A / limit;

			double r = color.R / limit;
			double g = color.G / limit;
			double b = color.B / limit;

			double max = Math.Max(Math.Max(r, g), b);
			double min = Math.Min(Math.Min(r, g), b);

			double diff = max - min;
			res.brightness = 0.5 * (max + min);
			res.hue = max == min ? 0 :
				(max == r && g >= b) ? 60 * (g - b) / diff :
				(max == r && g < b) ? 60 * (g - b) / diff + 360 :
				max == g ? 60 * (b - r) / diff + 120 : 60 * (r - g) / diff + 240;
			double l = res.brightness;
			res.saturation = (l == 0 || max == min) ? 0 :
				(0 <= l && l <= 0.5) ? diff / (max + min) :
				(0.5 < l && l < 1) ? diff / (2 - 2 * l) : 1;

			return res;
		}

		public Color ToARGB() {
			double hue = this.hue / 360;
			double r = 0;
			double g = 0;
			double b = 0;
			if (saturation == 0.0) {
				r = g = b = (int)Math.Floor(brightness * 255.0 + 0.5);
			}
			else {
				double h = (hue - Math.Floor(hue)) * 6.0;
				double f = h - Math.Floor(h);
				double p = brightness * (1.0 - saturation);
				double q = brightness * (1.0 - saturation * f);
				double t = brightness * (1.0 - (saturation * (1.0 - f)));

				switch ((int)Math.Floor(h)) {
					case 0:
						ToARGB(brightness, t, p, out r, out g, out b);
						break;

					case 1:
						ToARGB(q, brightness, p, out r, out g, out b);
						break;

					case 2:
						ToARGB(p, brightness, t, out r, out g, out b);
						break;

					case 3:
						ToARGB(p, q, brightness, out r, out g, out b);
						break;

					case 4:
						ToARGB(t, p, brightness, out r, out g, out b);
						break;

					case 5:
						ToARGB(brightness, p, q, out r, out g, out b);
						break;
				}
			}
			return new Color { A = (byte)(alpha * 255 + 0.5), R = (byte)r, G = (byte)g, B = (byte)b };
		}
	}

	public static class ColorExtensions {
		public static HSBColor ToHSBColor(this Color color) {
			return HSBColor.FromARGB(color);
		}
	}
}
