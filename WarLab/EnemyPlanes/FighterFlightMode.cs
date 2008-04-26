using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyPlanes
{
	/// <summary>
	/// Режим полета вражеского истребителя
	/// </summary>
	public enum EnemyFighterFlightMode
	{
		/// <summary>
		/// Сопровождение бомбардировщика
		/// </summary>
		FollowBomber,
		/// <summary>
		/// атака истребителя обороняющейся стороны
		/// </summary>
		Attack,
		/// <summary>
		/// возвращение на базу
		/// </summary>
		ReturnToBase,
		/// <summary>
		/// Ждать бомардировщика перед входом в зону РЛС
		/// </summary>
		WaitForBomber
	}
}
