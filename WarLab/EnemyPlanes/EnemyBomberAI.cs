using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab;

namespace EnemyPlanes
{
	[Controls(typeof(EnemyBomber))]
	public class EnemyBomberAI:EnemyPlaneAI
	{
		#region vars
		private BomberFlightMode mode;//режим
		private StaticTarget target;//бомбардируемая цель
		private double figtersRadius;
		public delegate void TargetReachedDelegate(Args args);
		public event TargetReachedDelegate targetReached;
		#endregion

		//public EnemyBomberAI(Vector3D basePosition):base(basePosition)
		//{
		//}

		/// <summary>
		/// Создать интеллект вражеского бомбера
		/// </summary>
		public EnemyBomberAI()
		{
			mode = BomberFlightMode.MoveToTarget;
		}

		#region properties


		/// <summary>
		/// Радиус круга, в котором должны находится обороняющие бомардировщик 
		/// истребители
		/// </summary>
		public double FightersRadius
		{
			get { return figtersRadius; }
			set { figtersRadius = value; }
		}


		/// <summary>
		/// Возвращает режим полета
		/// </summary>
		public BomberFlightMode Mode
		{
			get { return mode; }
			set { mode = value; }
		}

		/// <summary>
		/// Возвращает бомбардируемую цель
		/// </summary>
		public StaticObject Target
		{
			get { return target; }
		}

		#endregion

		#region EnemyPlaneAI Implementation
		
		public override void Update(WarTime time)
		{
			EnemyBomber plane = (EnemyBomber)ControlledPlane;
			//Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
			//	* plane.Speed;
			if (MathHelper.Distance(plane.Position, target.Position) <= 10.0)
			{
				if(targetReached!=null)
					targetReached(new Args((EnemyBomber)ControlledPlane,target));
			}
			else
				MoveInDirectionOf(target.Position);
		}

		#endregion


		#region Methods

		/// <summary>
		/// Указать цель для бомбардирования. Возвращает false, если самолет летит на базу
		/// </summary>
		/// <param name="target">статичный объект на земле</param>
		/// <returns>Удалось ли установить цель для бомбардирования</returns>
		public bool AttackTarget(StaticTarget target)
		{
			if (mode != BomberFlightMode.ReturnToBase)
			{
				this.target = target;
				return true;
			}
			return false;
		}

		#endregion

	}
}
