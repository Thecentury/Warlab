using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab;
using System.Diagnostics;
using WarLab.WarObjects;

namespace EnemyPlanes {
	public class EnemyFighterAI : FighterAI<Plane> {
		private EnemyFighterFlightMode mode = EnemyFighterFlightMode.FollowBomber;
		private double offsetDegree;

		#region properties
		/// <summary>
		/// Возвращает режим полета
		/// </summary>
		public EnemyFighterFlightMode Mode {
			get { return mode; }
			private set {
				mode = value;
				if (value == EnemyFighterFlightMode.ReturnToBase) {
					TimeSpan toBaseDuration = TimeSpan.FromSeconds(ControlledPlane.Airport.Position.DistanceTo(ControlledPlane.Position) / ControlledPlane.Speed);
					returnToBaseTime = toBaseDuration + World.Instance.Time.TotalTime;
				}
			}
		}


		/// <summary>
		/// Угол смещения от местонахождения бомбардировщика. В полученной точке должен
		/// лететь истребитель
		/// </summary>
		public double OffsetDegree {
			get { return offsetDegree; }
		}

		#endregion

		private EnemyBomber bomber;
		public override void Update(WarTime time) {
			if (TargetPlane != null) {
				if (mode != EnemyFighterFlightMode.ReturnToBase) {
					bool canContinue = CanContinue(time);
					if (!canContinue) {
						Mode = EnemyFighterFlightMode.ReturnToBase;
						Debug.WriteLine("Истребитель возвращается домой");
					}
				}

				bool canBeginAttack = CanBeginAttack;
				if ((Mode == EnemyFighterFlightMode.FollowBomber || Mode == EnemyFighterFlightMode.WaitForBomber) && canBeginAttack && CanRetarget) {
					Mode = EnemyFighterFlightMode.Attack;
					bomber = (EnemyBomber)TargetPlane;
					TargetPlane = closestFighter;
				}
				else if (!canBeginAttack && Mode == EnemyFighterFlightMode.Attack) {
					TargetPlane = bomber;
					Mode = EnemyFighterFlightMode.FollowBomber;
				}

				Vector3D flyTo = TargetPlane.Position;

				switch (mode) {
					case EnemyFighterFlightMode.ReturnToBase:
						flyTo = AirportPosition;
						ReturnToBase(time);
						break;
					case EnemyFighterFlightMode.FollowBomber:
						flyTo = FollowBomber(time);
						break;
					case EnemyFighterFlightMode.Attack:
						flyTo = FollowTarget(time);
						break;
					default:
						break;
				}

				MoveInDirectionOf(flyTo);
			}
		}

		private bool CanRetarget {
			get {
				return Mode != EnemyFighterFlightMode.ReturnToBase ||
				Mode == EnemyFighterFlightMode.ReturnToBase && Aim == ReturnToBaseAim.NoTargets;
			}
		}

		private OurFighter closestFighter;
		private bool CanBeginAttack {
			get {
				var ourFighters = World.Instance.SelectAll<OurFighter>().ToList();

				if (ourFighters.Count == 0) return false;

				ourFighters.Sort((f1, f2) => f1.Position.Distance2D(Position).CompareTo(f2.Position.Distance2D(Position)));

				closestFighter = ourFighters[0];
				// минимальное расстояние до нашего самолета
				double minDistance = ourFighters[0].Position.Distance2D(Position);
				return minDistance < 2 * attackDistance;
			}
		}

		/// <summary>
		/// Установить цель: либо бомбардировщик, либо истребитель
		/// </summary>
		/// <param name="t">Местоположение цели</param>
		/// <returns>удалось ли установить</returns>
		private bool SetTarget(Plane target) {
			if (mode != EnemyFighterFlightMode.ReturnToBase) {
				if (target is OurFighter) { }
				TargetPlane = target;
				EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
				plane.Speed = plane.MaxSpeed;
				return true;
			}
			return false;
		}

		#region Methods
		/// <summary>
		/// Определить, можем ли лететь дальше, или пора лететь на базу для дозаправки
		/// или перезарядки
		/// </summary>
		/// <returns></returns>
		private bool CanContinue(WarTime warTime) {
			EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
			
			if (plane.Orientation != Vector3D.Zero) {
				//насколько мы улетим по направлению к цели, если продолжим двигаться к ней
				Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
					* plane.Speed;

				/*если после этого мы не сможем вернутся домой, то возвращаемся обратно
				Другое условие возвращения - кончились ракеты. Но, поскольку CanContinue в
				 * вызывается в Update до MoveInDirectionOf, первый раз когда происходит
				 * Update вектор Orientation нулевой (потому что именно MoveInDirectionOf
				 * его меняет). Поэтому сначала есть проверка на ненулевой вектор
				 */
				if (MathHelper.Distance(plane.Position + shift, AirportPosition) > plane.FuelLeft ||
					plane.WeaponsLeft < 1) {

					Mode = EnemyFighterFlightMode.ReturnToBase;
					Aim = ReturnToBaseAim.ReloadOrRefuel;
					
					plane.Speed = plane.MaxSpeed; //домой летим на максимальной скорости
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase(WarTime time) {
			if (ShouldLand(time)) {
				LandPlane();
				if (TargetPlane is EnemyBomber) {
					DetachFromBomberEvents((EnemyBomber)TargetPlane);
				}
			}
		}

		/// <summary>
		/// Лететь за бомбардировщиком
		/// <param name="warTime"></param>
		/// <returns>новая точка, где должен оказаться истребитель</returns>
		/// </summary>
		private Vector3D FollowBomber(WarTime warTime) {
			EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
			EnemyBomber bomber = (EnemyBomber)TargetPlane;

			//насколько мы улетим по направлению к бомбардировщику
			double shift = 10; //warTime.ElapsedTime.TotalSeconds * plane.Speed;

			double orientationAngle = 180.0 *
				Math.Atan2(bomber.Orientation.Y, bomber.Orientation.X) / Math.PI;// угол между направлением на восток и вектором ориентации бомбера
			double newAngle = offsetDegree + orientationAngle;/* угол между направлением на восток 
				и местом, где должен оказаться истребитель = угол между направление бомбера 
				и направленим на восток (orientationAngle)+ угол смещения относительно 
				бомбера (offsetDegree)*/

			newAngle = MathHelper.AngleToRadians(newAngle);

			Vector3D offsetVector = new Vector3D(Math.Cos(newAngle),
				Math.Sin(newAngle)); /* вектор направления смещения 
													    * (как Orientation у бомбера)*/

			// точка на окружности вокруг бомбера, где должен оказаться истребитель
			Vector3D newPosition = bomber.Position + ((EnemyBomberAI)bomber.AI).FightersRadius *
				offsetVector;

			double distance = plane.Position.Distance2D(newPosition);
			if (distance <= shift) {
				// если мы за один такт подлетим к бомберу, снижаем скорость
				plane.Speed = bomber.Speed;
			}
			else
				if (plane.Speed < plane.MaxSpeed) {
					/* мы уже летели на скорости бомбера (потому что Speed<MaxSpeed),
					но мы отстали от него (потому что if (distance < shift.Length) не 
					 * сработал, а это значит, что за один такт нам не долететь до бомбера*/
					plane.Speed = Math.Min(2 * bomber.Speed, (distance * plane.Speed / shift));

					/* увеличиваем 
						скорость, чтоб догнать бомбера*/
				}
			if (plane.Speed > plane.MaxSpeed) {
				// чтоб избегать телепорта, когда бомбардировщик меняет направление
				plane.Speed = plane.MaxSpeed;
			}

			return newPosition;
		}

		/// <summary>
		/// Указать, какого бомбардировщика сопровождать. Возвращает false, если самолет летит на базу
		/// </summary>
		/// <param name="bomber">Сопровождаемый бомбер</param>
		/// <param name="OffsetDegree">Угол смещения от местанахождения бомбера</param>
		/// <returns>Удалось ли установить бомбера и перейти в режим сопровождения</returns>
		public bool FollowBomber(EnemyBomber bomber, double offsetDegree) {
			this.offsetDegree = offsetDegree;

			EnemyBomberAI bomberAI = (EnemyBomberAI)bomber.AI;
			bomberAI.Landed += OnBomberLanded;

			bomber.Destroyed += OnBomberDestroyed;

			return SetTarget(bomber);
		}

		private void OnBomberDestroyed(object sender, EventArgs e) {
			EnemyBomber bomber = (EnemyBomber)sender;
			DetachFromBomberEvents(bomber);
			Mode = EnemyFighterFlightMode.ReturnToBase;
		}

		private void OnBomberLanded(object sender, LandingEventArgs e) {
			if (e.LandingAim == ReturnToBaseAim.NoTargets) {
				EnemyBomberAI bomberAI = (EnemyBomberAI)sender;
				DetachFromBomberEvents((EnemyBomber)bomberAI.ControlledPlane);
				Mode = EnemyFighterFlightMode.ReturnToBase;
			}
		}

		private void DetachFromBomberEvents(EnemyBomber bomber) {
			bomber.Destroyed -= OnBomberDestroyed;
			((EnemyBomberAI)bomber.AI).Landed -= OnBomberLanded;
		}

		/// <summary>
		/// Атаковать истребитель обороняющейся стороны. Возвращает false, если самолет летит на базу
		/// </summary>
		/// <param name="fighter">истребитель</param>
		/// <returns>Удалось ли перейти в режим атаки</returns>
		public bool AttackFighter(Plane fighter) {
			return SetTarget(fighter);
		}



		#endregion

		protected override void BeginReturnToBase() {
			Mode = EnemyFighterFlightMode.ReturnToBase;
		}
	}
}
