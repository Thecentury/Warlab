using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyPlanes
{
	/// <summary>
	/// Режим полета вражеского бомбардировщика
	/// </summary>
	public enum BomberFlightMode
	{
		/// <summary>
		/// Вернуться на аэропорт на базу для перезаправки или установки бомб
		/// </summary>
		ReturnToBase,
		/// <summary>
		/// Лететь к цели для ее бомбардирования
		/// </summary>
		MoveToTarget
	}
}
