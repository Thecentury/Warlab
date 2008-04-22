using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace WarLab {
	public static class Verify {
		public static void IsFinite(double d) {
			if (Double.IsNaN(d) || Double.IsInfinity(d)) {
				Debugger.Break();
				throw new ArgumentException("Величина не может быть равной NAN или бесконечности");
			}
		}

		public static void IsNonNegative(double d) {
			IsFinite(d);
			if (d < 0) {
				Debugger.Break();
				throw new ArgumentOutOfRangeException("Величина не может быть отрицательной");
			}
		}

		public static void IsPositive(double d) {
			IsFinite(d);
			if (d <= 0) {
				Debugger.Break();
				throw new ArgumentOutOfRangeException("Величина не может быть отрицательной");
			}
		}

		public static void IsInSegment(double value, double min, double max) {
			if (!(min <= value && value <= max)) {
				Debugger.Break();
				throw new ArgumentOutOfRangeException("Value должно быть больше min и меньше max.");
			}
		}

		public static void Is0To1(double d) {
			if (!(0 <= d && d <= 1)) {
				Debugger.Break();
				throw new ArgumentException("Величина должна быть от 0 до 1");
			}
		}

		public static void IsNonNegative(int i) {
			if (i < 0) {
				throw new ArithmeticException("Величина не может быть неотрицательной");
			}
		}

		public static void IsTrue(bool condition) {
			if (!condition) {
				Debugger.Break();
				throw new ArgumentException("Условие не выполнено.");
			}
		}
	}
}
