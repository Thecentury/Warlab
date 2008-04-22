using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public interface ITimeControl {
		/// <summary>
		/// Возвращает или устанавливает скорость времени в мире.
		/// </summary>
		double Speed { get; set; }
		/// <summary>
		/// Начинает или возобновляет обновление времени в мире.
		/// </summary>
		void Start();
		/// <summary>
		/// Останавливает обновление времени в мире.
		/// </summary>
		void Stop();
		/// <summary>
		/// Течет ли время в мире?
		/// </summary>
		bool IsRunning { get; }
	}
}
