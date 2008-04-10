using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;

namespace EnemyPlanes
{
	public class EnemyBomber:EnemyPlane
	{
		#region vars
		#endregion

		/// <summary>
		/// Создать вражеский бомбардировщик
		/// </summary>
		/// <param name="bombs">Количество переносимых бомб</param>
		/// <param name="fuel">Вместимость бака с топливом</param>
		/// <param name="speed">Скорость по умолчанию</param>
		/// бомбардировщики</param>
		public EnemyBomber(int bombs, double fuel,double speed):base(bombs,fuel,speed)
		{	
		
		}

		/// <summary>
		/// Создать вражеский бомбардировщик
		/// </summary>
		/// <param name="bombs">Количество переносимых бомб</param>
		/// <param name="fuel">Вместимость бака с топливом</param>
		/// бомбардировщики</param>
		public EnemyBomber(int bombs, double fuel)
			: base(bombs, fuel) 
		{
			
		}
	
		#region properties

		
		#endregion

		#region Plane implementation
		protected override void UpdateCore(WarTime warTime)
		{
		}
		#endregion
	}
}
