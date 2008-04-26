using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;
using System.Diagnostics;

namespace WarLab.AI {
	public sealed class OurFighterAI : PlaneAI {

		private TimeSpan returnToBaseTime;

		private OurFighterFlightMode mode = OurFighterFlightMode.Attack;
		public OurFighterFlightMode Mode {
			get { return mode; }
			private set {
				mode = value;
			}
		}

		private EnemyPlane targetPlane;
		public EnemyPlane TargetPlane {
			get { return targetPlane; }
		}

		public override void Update(WarTime time) {
			if (targetPlane != null) {
				Vector3D flyTo = targetPlane.Position;
				if (mode != OurFighterFlightMode.ReturnToBase) {
					bool canContinue = CanContinue(time);

					if (!canContinue) {
						Mode = OurFighterFlightMode.ReturnToBase;
						Debug.WriteLine("Истребитель возвращается домой");
					}
				}
				switch (mode) {
					case OurFighterFlightMode.ReturnToBase:
						flyTo = AirportPosition;
						ReturnToBase(time);
						break;
					case OurFighterFlightMode.Attack:
						flyTo = FollowTarget(time);
						break;
					default:
						break;
				}

				MoveInDirectionOf(flyTo);
			}
		}

		private Vector3D FollowTarget(WarTime time) {
			Vector3D position = targetPlane.Position;
			return position;
		}


		/// <summary>
		/// Установить цель: либо бомбардировщик, либо истребитель
		/// </summary>
		/// <param name="t">Местоположение цели</param>
		/// <returns>удалось ли установить</returns>
		private bool SetTarget(EnemyPlane targetPlane) {
			if (mode != OurFighterFlightMode.ReturnToBase) {
				this.targetPlane = targetPlane;
				// todo подписываться на события.
				return true;
			}
			return false;
		}

		/// <summary>
		/// Определить, можем ли лететь дальше, или пора лететь на базу для дозаправки
		/// или перезарядки
		/// </summary>
		/// <returns></returns>
		private bool CanContinue(WarTime warTime) {
			Plane plane = (OurFighter)ControlledPlane;

			//насколько мы улетим по направлению к цели, если продолжим двигаться к ней
			Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
				* plane.Speed;

			//если после этого мы не сможем вернутся домой, то возвращаемся обратно
			//Другое условие возвращения - кончились ракеты. 
			if ((plane.Position + shift).DistanceTo(AirportPosition) > plane.FuelLeft ||
				plane.WeaponsLeft < 1) {

				Mode = OurFighterFlightMode.ReturnToBase;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase(WarTime time) {
			if (returnToBaseTime < time.TotalTime) {
				LandPlane();
			}
		}

		/// <summary>
		/// Атаковать вражеский самолет.
		/// </summary>
		/// <param name="plane"></param>
		/// <returns></returns>
		public bool AttackFighter(EnemyPlane plane) {
			return SetTarget(plane);
		}

	}
}
