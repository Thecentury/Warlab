using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;

namespace EnemyPlanes {
	/// <summary>
	/// Базовый класс для вражеских самолетов
	/// </summary>
	public abstract class EnemyPlane : Plane {
		#region vars
		protected readonly double tankCapacity = 0;//Вместимость бензобака
		protected readonly int weaponsCapacity = 0;//Сколько может нести вооружения (бомб или ракет)
		protected int weaponsLeft;//сколько осталось оружия
		#endregion

		public EnemyPlane(int weapons, double fuel, double speed) {
			FuelLeft = fuel;//нету public setter'а в этом свойстве
			this.Speed = speed;
			this.tankCapacity = fuel;
			this.weaponsLeft = weapons;
			this.weaponsCapacity = weapons;
		}

		public EnemyPlane(int weapons, double fuel) {
			FuelLeft = fuel;//нету public setter'а в этом свойстве
			this.tankCapacity = fuel;
			this.weaponsLeft = weapons;
			this.weaponsCapacity = weapons;
		}

		#region Properties

		/// <summary>
		/// Возвращает вместимость бака с топливом
		/// </summary>
		public double TankCapacity {
			get { return tankCapacity; }
		}
		/// <summary>
		/// Количество оставшегося вооружения. Когда его 0 - надо лететь на базу
		/// </summary>
		public int WeaponsLeft {
			get { return weaponsLeft; }
			set { weaponsLeft = value; }
		}

		/// <summary>
		/// Возвращает количество оружия, которое может нести истребитель
		/// </summary>
		public int WeaponsCapacity {
			get { return weaponsCapacity; }
		}

		#endregion
	}
}
