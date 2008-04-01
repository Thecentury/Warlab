using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Просто статический датчик случайных чисел. Нужен потому, что несколько экземпляров
	/// класса Random, созданные последовательно с малыми задержками между созданиями,
	/// начинают выдавать одинаковые последовательности.
	/// </summary>
	public static class StaticRandom {
		private static readonly Random rnd = new Random();

		/// <summary>
		/// Возвращает случайное число в интервале от 0 до 1.
		/// </summary>
		/// <returns></returns>
		public static double NextDouble() {
			return rnd.NextDouble();
		}
	}
}
