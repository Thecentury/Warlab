using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;
using System.Diagnostics;

namespace WarLab.AI {
	public sealed class OurFighterAI : FighterAI<EnemyPlane> {

		private OurFighterFlightMode mode = OurFighterFlightMode.Attack;
		public OurFighterFlightMode Mode {
			get { return mode; }
			private set {
				mode = value;
				if (value == OurFighterFlightMode.ReturnToBase) {
#if false
					SetReturnToBaseTime();
#else
					LandPlane();
#endif
				}
			}
		}

		public override void Update(WarTime time) {
			//if (TargetPlane == null) 
			//    return;

			Vector3D flyTo = new Vector3D();
			if (TargetPlane != null) {
				flyTo = TargetPlane.Position;
			}
			else {
				Mode = OurFighterFlightMode.ReturnToBase;
			}
			if (mode != OurFighterFlightMode.ReturnToBase) {
				if (TargetPlane != null) {
					bool canContinue = CanContinue(time);

					if (!canContinue) {
						//Mode = OurFighterFlightMode.ReturnToBase;
						//aim = ReturnToBaseAim.ReloadOrRefuel;

						Debug.WriteLine("Истребитель возвращается домой - " + Aim);
					}
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
			};

			Plane plane = (OurFighter)ControlledPlane;

			// возврат на базу если нет РЛС или если вне зоны действия РЛС - сам или цель 
			var rlses = World.Instance.SelectAll<RLS>().ToArray();
			bool shouldReturnDueRLS = rlses.Length == 0 ||
				rlses.Length > 0 &&
				(!rlses.Any(rls => rls.IsInCoverage(plane.Position)) ||
				!rlses.Any(rls => rls.IsInCoverage(TargetPlane.Position)));

			if (shouldReturnDueRLS) {
				Mode = OurFighterFlightMode.ReturnToBase;
				Aim = ReturnToBaseAim.NoTargets;
				return false;
			}


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

			if (CanRetarget(plane)) {
				TargetPlane = plane;
				Mode = OurFighterFlightMode.Attack;
				Aim = ReturnToBaseAim.NoTargets;
				return true;
			}
			return false;
		}

		public bool CanRetarget(EnemyPlane anotherPlane) {
			return mode == OurFighterFlightMode.ReturnToBase && Aim == ReturnToBaseAim.NoTargets
				|| TargetPlane == null
				|| mode == OurFighterFlightMode.Attack &&
				(TargetPlane.PlaneImportance <= anotherPlane.PlaneImportance || TargetPlane.Health <= 0);
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
