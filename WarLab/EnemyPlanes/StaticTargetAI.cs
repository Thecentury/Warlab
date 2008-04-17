using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;

namespace EnemyPlanes
{
	[Controls(typeof(StaticTarget))]
	public class StaticTargetAI:WarAI
	{

		/// <summary>
		/// Радиус окружности вокруг объекта, при попадании в которую бомбы считаем, что 
		/// попали в объект
		/// </summary>
		private double damageRadius = 10.0;

		public double DamageRadius
		{
			get { return damageRadius; }
			set { damageRadius = value; }
		}

		public override void Update(WarLab.WarTime time)
		{
			
		}
	}
}
