using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using WarLab.AI;

namespace EnemyPlanes
{
	public abstract class EnemyPlaneAI: DynamicObjectAI
	{
		#region vars
		//private Vector3D target;//цель, которую надо бомбардировать

		protected  Vector3D basePosition;//где база

		#endregion

		//public EnemyPlaneAI(Vector3D basePosition)
		//{
		//    this.basePosition = basePosition;
		//}

		public EnemyPlaneAI()
		{
		}

		#region properties
		///// <summary>
		///// Возвращает бомбардиуемую цель
		///// </summary>
		//public Vector3D Target
		//{
		//    get { return target; }
		//    //set { target = value; }
		//}
		/// <summary>
		/// Возвращает местоположение базы
		/// </summary>
		public Vector3D BasePosition
		{
			get { return basePosition; }
			set { basePosition = value; }
		}

		#endregion

		#region methods


		///// <summary>
		///// Установить цель
		///// </summary>
		///// <param name="t">Местонахождение цели</param>
		///// <returns></returns>
		//protected virtual bool SetTarget(Vector3D t)
		//{
		//    this.target = t;
		//    return true;
		//}
		
		#endregion
	}
}
