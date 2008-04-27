﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;
using System.Diagnostics;

namespace WarLab.AI {
	public sealed class OurFighterAI : FighterAI<EnemyPlane> {
		public OurFighterAI() {
		}

		private OurFighterFlightMode mode = OurFighterFlightMode.Attack;
		public OurFighterFlightMode Mode {
			get { return mode; }
			private set {
				mode = value;
				if (value == OurFighterFlightMode.ReturnToBase) {
					SetReturnToBaseTime();
				}
			}
		}

		public override void Update(WarTime time) {
			Vector3D flyTo = TargetPlane.Position;
			if (mode != OurFighterFlightMode.ReturnToBase) {
				bool canContinue = CanContinue(time);

				if (!canContinue) {
					//Mode = OurFighterFlightMode.ReturnToBase;
					//aim = ReturnToBaseAim.ReloadOrRefuel;

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

		/// <summary>
		/// Определить, можем ли лететь дальше, или пора лететь на базу для дозаправки
		/// или перезарядки
		/// </summary>
		/// <returns></returns>
		private bool CanContinue(WarTime warTime) {
			if (TargetPlane.Health <= 0) {
				Mode = OurFighterFlightMode.ReturnToBase;
				Aim = ReturnToBaseAim.NoTargets;
				return false;
			}

			Plane plane = (OurFighter)ControlledPlane;

			//насколько мы улетим по направлению к цели, если продолжим двигаться к ней
			Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
				* plane.Speed;

			//если после этого мы не сможем вернутся домой, то возвращаемся обратно
			//Другое условие возвращения - кончились ракеты. 
			if ((plane.Position + shift).Distance2D(AirportPosition) > plane.FuelLeft ||
				plane.WeaponsLeft < 1) {

				Mode = OurFighterFlightMode.ReturnToBase;
				Aim = ReturnToBaseAim.ReloadOrRefuel;
				return false;
			}
			return true;
		}

		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase(WarTime time) {
			if (ShouldLand(time)) {
				LandPlane();
				Mode = OurFighterFlightMode.Attack;
				Aim = ReturnToBaseAim.NoTargets;
			}
		}

		/// <summary>
		/// Атаковать вражеский самолет.
		/// </summary>
		/// <param name="plane"></param>
		/// <returns></returns>
		public bool AttackTarget(EnemyPlane plane) {
			if (plane == null)
				throw new ArgumentNullException("plane");

			if (CanRetarget) {
				TargetPlane = plane;
				return true;
			}
			return false;
		}

		public bool CanRetarget {
			get {
				return mode == OurFighterFlightMode.ReturnToBase && Aim == ReturnToBaseAim.NoTargets
					|| //mode == OurFighterFlightMode.Attack ||
					TargetPlane == null;
			}
		}

		internal void ReturnToBase() {
			Mode = OurFighterFlightMode.ReturnToBase;
			Aim = ReturnToBaseAim.NoTargets;
		}

		protected override void BeginReturnToBase() {
			Mode = OurFighterFlightMode.ReturnToBase;
		}

		protected override Side GetSide() {
			return Side.Our;
		}
	}
}
