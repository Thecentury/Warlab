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
			this.MaxFuel = fuel;
			this.Speed = speed;
			this.weaponsLeft = weapons;
			this.weaponsCapacity = weapons;
		}

		public EnemyPlane(int weapons, double fuel) {
			this.MaxFuel = fuel;
			this.weaponsLeft = weapons;
			this.weaponsCapacity = weapons;
		}

		#region Properties

		public void Reload() {
			WeaponsLeft = WeaponsCapacity;
		}

		/// <summary>
		/// Количество оставшегося вооружения. Когда его 0 - надо лететь на базу
		/// </summary>
		public int WeaponsLeft {
			get { return weaponsLeft; }
			private set {
				Verify.IsNonNegative(value);

				weaponsLeft = value;
			}
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
