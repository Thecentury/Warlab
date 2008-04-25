using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab;
using System.Diagnostics;

namespace EnemyPlanes {
	public class EnemyFighterAI : EnemyPlaneAI {
		private Plane target;
		private FighterFlightMode mode = FighterFlightMode.FollowBomber;
		private double offsetDegree;

		private TimeSpan returnToBaseTime;
		#region properties
		/// <summary>
		/// Возвращает режим полета
		/// </summary>
		public FighterFlightMode Mode {
			get { return mode; }
			private set {
				mode = value;
				if (value == FighterFlightMode.ReturnToBase) {
					TimeSpan toBaseDuration = TimeSpan.FromSeconds(ControlledPlane.Airport.Position.LengthTo(ControlledPlane.Position) / ControlledPlane.Speed);
					returnToBaseTime = toBaseDuration + World.Instance.Time.TotalTime;
				}
			}
		}

		/// <summary>
		/// Возвращает цель (сопровождаемый бомбардировщик или истребитель обороняющейся
		/// стороны
		/// </summary>
		public Plane Target {
			get { return target; }
		}

		/// <summary>
		/// Угол смещения от местонахождения бомбардировщика. В полученной точке должен
		/// лететь истребитель
		/// </summary>
		public double OffsetDegree {
			get { return offsetDegree; }
			//set { offsetDegree = value; }
		}

		#endregion

		#region EnemyPlaneAI implementation
		public override void Update(WarTime time) {
			if (target != null) {
				Vector3D flyTo = target.Position;
				if (mode != FighterFlightMode.ReturnToBase) {
					bool canContinue = CanContinue(time);
					if (!canContinue) {
						Mode = FighterFlightMode.ReturnToBase;
						Debug.WriteLine("Истребитель возвращается домой");
					}
				}

				switch (mode) {
					case FighterFlightMode.ReturnToBase:
						flyTo = AirportPosition;
						ReturnToBase();
						break;
					case FighterFlightMode.FollowBomber:
						flyTo = FollowBomber(time);
						break;
					default: break;

				}
				MoveInDirectionOf(flyTo);
			}
		}


		/// <summary>
		/// Установить цель: либо бомбардировщик, либо истребитель
		/// </summary>
		/// <param name="t">Местоположение цели</param>
		/// <returns>удалось ли установить</returns>
		private bool SetTarget(Plane t) {
			if (mode != FighterFlightMode.ReturnToBase) {
				target = t;
				EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
				plane.Speed = plane.MaxSpeed;
				return true;
			}
			return false;
		}

		#endregion

		#region Methods
		/// <summary>
		/// Определить, можем ли лететь дальше, или пора лететь на базу для дозаправки
		/// или перезарядки
		/// </summary>
		/// <returns></returns>
		private bool CanContinue(WarTime warTime) {
			EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
			if (plane.Orientation.X != 0 && plane.Orientation.Y != 0 &&
				plane.Orientation.H != 0) {
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
					
					Mode = FighterFlightMode.ReturnToBase;
					plane.Speed = plane.MaxSpeed; //домой летим на максимальной скорости
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase() {
			if (returnToBaseTime < World.Instance.Time.TotalTime) {
				LandPlane();
				DetachFromBomberEvents((EnemyBomber)target);
			}
			//EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
			////насколько мы улетим по направлению к базе, если продолжим двигаться к ней
			//Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
			//    * plane.Speed;
			//// если это смещение больше расстояния до базы, то садимся на базу
			//if (MathHelper.Distance(plane.Position, basePosition) < shift.Length)
			//{
			//    plane.Position = basePosition;
			//}
			//target = basePosition;
		}

		/// <summary>
		/// Лететь за бомбардировщиком
		/// <param name="warTime"></param>
		/// <returns>новая точка, где должен оказаться истребитель</returns>
		/// </summary>
		private Vector3D FollowBomber(WarTime warTime) {
			EnemyFighter plane = (EnemyFighter)ControlledDynamicObject;
			EnemyBomber bomber = (EnemyBomber)target;

			//насколько мы улетим по направлению к бомбардировщику
			Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
				* plane.Speed;
			Vector3D newPosition = new Vector3D();// точка на окружности вокруг бомбера, где должен оказаться истребите
			double orientationAngle = 180.0 *
				Math.Atan2(bomber.Orientation.Y, bomber.Orientation.X) / Math.PI;// угол между направлением на восток и вектором ориентации бомбера
			double newAngle = offsetDegree + orientationAngle;/* угол между направлением на восток 
				и местом, где должен оказаться истребитель = угол между направление бомбера 
				и направленим на восток (orientationAngle)+ угол смещения относительно 
				бомбера (offsetDegree)*/
			Vector3D offsetVector = new Vector3D(Math.Cos(newAngle * Math.PI / 180.0),
				Math.Sin(newAngle * Math.PI / 180.0), 0.0); /* вектор направления смещения 
													    * (как Orientation у бомбера)*/
			newPosition = bomber.Position + ((EnemyBomberAI)bomber.AI).FightersRadius *
				offsetVector;

			double distance = (MathHelper.Distance(plane.Position, newPosition));
			if (distance <= shift.Length) {
				// если мы за один такт подлетим к бомберу, снижаем скорость
				plane.Speed = bomber.Speed;
			}
			else
				if (plane.Speed < plane.MaxSpeed) {
					/* мы уже летели на скорости бомбера (потому что Speed<MaxSpeed),
					но мы отстали от него (потому что if (distance < shift.Length) не 
					 * сработал, а это значит, что за один такт нам не долететь до бомбера*/
					plane.Speed = 1.5 * bomber.Speed; // (distance * plane.Speed / shift.Length);
					
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
			Mode = FighterFlightMode.ReturnToBase;
		}

		private void OnBomberLanded(object sender, LandingEventArgs e) {
			if (e.LandingAim == LandingAim.NoTargets) {
				EnemyBomberAI bomberAI = (EnemyBomberAI)sender;
				DetachFromBomberEvents((EnemyBomber)bomberAI.ControlledPlane);
				Mode = FighterFlightMode.ReturnToBase;
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


	}
}
