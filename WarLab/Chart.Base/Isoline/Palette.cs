using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Media;

namespace ScientificStudio.Charting.Isoline {
	public interface IPalette {
		Color GetColor(double t);
	}

	public sealed class HSBPalette : IPalette {
		private double start = 0;
		[DefaultValue(0.0)]
		public double PaletteStart {
			get { return start; }
			set { start = value; }
		}

		private double width = 360;
		[DefaultValue(360.0)]
		public double PaletteWidth {
			get { return width; }
			set { width = value; }
		}

		#region IPalette Members

		public Color GetColor(double t) {
			ChartDebug.AssertDoubleNNaN(t);
			Debug.Assert(0 <= t && t <= 1);

			return new HSBColor(start + t * width, 1, 1).ToARGB();
		}

		#endregion
	}

	public sealed class LinearPalette : IPalette {
		private double[] points;
		private readonly Color[] colors;

		public LinearPalette(params Color[] colors) {
			if (colors == null) throw new ArgumentNullException("colors");
			if (colors.Length < 2) throw new ArgumentException("Length of colors should be greater or equal to 2");

			this.colors = colors;
			FillPoints(colors.Length);
		}

		public LinearPalette(int size) {
			Debug.Assert(size > 1);

			colors = new Color[size];
			FillPoints(size);
		}

		private void FillPoints(int size) {
			points = new double[size];
			for (int i = 0; i < size; i++)
				points[i] = i / (double)(size - 1);
		}

		public Color GetColor(double t) {
			ChartDebug.AssertDoubleNNaN(t);

			if (t <= 0)
				return colors[0];
			else if (t >= 1)
				return colors[colors.Length - 1];
			else {
				int i = 0;
				while (points[i] < t)
					i++;
				Debug.Assert(0 < i && i < colors.Length);

				double alpha = (points[i] - t) / (points[i] - points[i - 1]);

				Debug.Assert(0 <= alpha && alpha <= 1);
				Color c0 = colors[i - 1];
				Color c1 = colors[i];
				Color res = Color.FromRgb((byte)(c0.R * alpha + c1.R * (1 - alpha)),
					(byte)(c0.G * alpha + c1.G * (1 - alpha)),
					(byte)(c0.B * alpha + c1.B * (1 - alpha)));

				// Increasing sat. and bri.
				HSBColor hsb = res.ToHSBColor();
				hsb.Saturation = 0.5 * (1 + hsb.Saturation);
				hsb.Brightness = 0.5 * (1 + hsb.Brightness);
				return hsb.ToARGB();
			}
		}

		private static readonly LinearPalette blackAndWhitePalette = new LinearPalette(
			Colors.Black,
			Colors.White);
		public static LinearPalette BlackAndWhitePalette {
			get { return blackAndWhitePalette; }
		}

		private static readonly LinearPalette rgbPalette = new LinearPalette(
			Colors.Blue,
			Colors.Green,
			Colors.Red);
		public static LinearPalette RedGreenBluePalette {
			get { return rgbPalette; }
		}

		private static readonly LinearPalette blueOrangePalette = new LinearPalette(
			Colors.Blue,
			Colors.Cyan,
			Colors.Yellow,
			Colors.Orange);
		public static LinearPalette BlueOrangePalette {
			get { return LinearPalette.blueOrangePalette; }
		}
	}
}
