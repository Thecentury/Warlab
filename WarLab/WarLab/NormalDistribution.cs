using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Датчик случайных чисел с нормальным распределением.
	/// </summary>
	public sealed class NormalDistribution {
		Random rnd = new Random();

		const int len = 1200;
		public double Generate(double m, double sigma) {
			double sum = 0;
			for (int i = 0; i < len; i++) {
				sum += rnd.NextDouble();
			}
			sum -= len / 2;
			sum *= Math.Sqrt(1.0 / 12); // was len / 12
			sum = sum * sigma + m;
			return sum;
		}
	}
}
