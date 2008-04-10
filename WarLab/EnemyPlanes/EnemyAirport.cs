﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;

namespace EnemyPlanes
{
	/// <summary>
	/// Вражеский аэродром, на котором находятся либо бомбардировщики, либо истребители
	/// </summary>
	public class EnemyAirport : WarObject,ISpriteSource
	{
		#region vars
		/// <summary>
		/// Самолеты, взлетающие и заправляющиеся на этом аэродроме
		/// </summary>
		private List<EnemyPlane> planes;
		#endregion

		/// <summary>
		/// Создать вражеский аэродром
		/// </summary>
		/// <param name="capacity">Количество самолетов, приписанных к нему</param>
		public EnemyAirport(int capacity) 
		{
			planes = new List<EnemyPlane>(capacity);
		}

		/// <summary>
		/// Создать вражеский аэродром
		/// </summary>
		public EnemyAirport()
		{
			planes = new List<EnemyPlane>();
		}

		#region properties

		/// <summary>
		/// Возвращает самолеты, приписанные к этому вражескому аэродрому
		/// </summary>
		public List<EnemyPlane> Planes
		{
			get { return planes; }
		}

		#endregion

		#region WarObject implementation
		protected override void UpdateImpl(WarTime warTime)
		{
			
		}
		#endregion

		#region methods

		/// <summary>
		/// Заправить самолет и установить на него вооружение
		/// </summary>
		/// <param name="plane">Самолет</param>
		public void ReloadAndRefuel(EnemyPlane plane)
		{
			if (planes.Contains(plane))
			{
				plane.FuelLeft = plane.TankCapacity;
				plane.WeaponsLeft = plane.WeaponsCapacity;
			}
			else
				throw new ArgumentException("Этот самолет не приписан к этому аэродрому");
		}

		#endregion

		#region ISpriteSource Members

		public Vector3D Orientation
		{
			get { return new Vector3D(1.0,0.0,0.0); }
		}

		#endregion
	}
}
