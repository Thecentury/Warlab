using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab;
using WarLab.WarObjects;
using System.Diagnostics;

namespace EnemyPlanes {
	public class EnemyBomberAI : EnemyPlaneAI {
		#region vars
		/// <summary>
		/// Режим полета бомбера
		/// </summary>
		private BomberFlightMode mode;

		/// <summary>
		/// Бомбардируемая цель
		/// </summary>
		private StaticTarget target;

		/// <summary>
		/// Радиус окружности, в которой летят истребители рядом с бомберов
		/// </summary>
		private double figtersRadius;
		public delegate void TargetReachedDelegate(object sender, TargetReacherEventArgs args);
		public event TargetReachedDelegate TargetReached;
		/// <summary>
		/// Нижняя высота, на которую можно совершить маневр
		/// </summary>
		public static double minHeight;
		/// <summary>
		/// Верхняя высота, на которую можно совершить маневр
		/// </summary>
		public static double maxHeight;
		/// <summary>
		/// Надо ли делать маневр
		/// </summary>
		private bool willManeuver = true;
		/// <summary>
		/// Через сколько тактов делать маневр после входа в зону рлс
		/// </summary>
		private int maneuverTick;
		/// <summary>
		/// Через сколько тактов после входа в зону рлс минимум бомбардировщик
		/// делает маневр
		/// </summary>
		public static int minManeuverTick = 1;
		/// <summary>
		/// Через сколько тактов после входа в зону рлс максимум бомбардировщик 
		/// делает маневр
		/// </summary>
		public static int maxManeuverTick = 10;
		/// <summary>
		/// генератор случайных чисел
		/// </summary>
		private Random rand;
		/// <summary>
		/// Коэффициент, умножаемый на случайное число от 0 до 1. Полученное число определяет
		/// отклонение от цели, в метрах.
		/// </summary> 
		public static double errorCoef = 5.0;
		/// <summary>
		/// Радиус окружности, по которой надо пролетететь бомбардировщику, чтоб вернуться
		/// к бомбометанию
		/// </summary>
		public static double returnToTargetRadius = 50.0;
		/// <summary>
		/// Центр окружности, по которой надо пролететь бомберу, чтоб вернуться
		/// к бомбометанию
		/// </summary>
		private static Vector3D returnToTargetCircleCenter;
		#endregion

		//public EnemyBomberAI(Vector3D basePosition):base(basePosition)
		//{
		//}

		/// <summary>
		/// Создать интеллект вражеского бомбера
		/// </summary>
		public EnemyBomberAI() {
			Mode = BomberFlightMode.MoveToTarget;
			rand = StaticRandom.Random;
		}

		#region properties


		/// <summary>
		/// Радиус круга, в котором должны находится обороняющие бомардировщик 
		/// истребители
		/// </summary>
		public double FightersRadius {
			get { return figtersRadius; }
			set { figtersRadius = value; }
		}


		/// <summary>
		/// Возвращает режим полета
		/// </summary>
		public BomberFlightMode Mode {
			get { return mode; }
			set { mode = value; }
		}

		/// <summary>
		/// Возвращает бомбардируемую цель
		/// </summary>
		public StaticObject Target {
			get { return target; }
		}

		#endregion

		#region EnemyPlaneAI Implementation

		private Vector3D extrapolatedTargetPos;
		public override void Update(WarTime time) {
			EnemyBomber plane = (EnemyBomber)ControlledDynamicObject;
			if (target != null) {
				Vector3D flyTo = target.Position;

				if ((target.Position - plane.Position).Length > 0) {
					extrapolatedTargetPos = target.Position + plane.Orientation * plane.Speed * 0.5;
				}

				if (mode != BomberFlightMode.ReturnToBase) {
					bool canContinue = CanContinue(time);
					if (!canContinue)
						Mode = BomberFlightMode.ReturnToBase;
				}

				switch (mode) {
					case BomberFlightMode.ReturnToBase:
						flyTo = basePosition;
						ReturnToBase();
						break;
					case BomberFlightMode.MoveToTarget:
						if (CanBomb) {
							DropBomb();
							RaiseTargetReached(plane);
						}
						break;
					case BomberFlightMode.ReturnToBombTarget:
						ReturnToTarget();
						break;
					default:
						break;
				}

				if ((flyTo - plane.Position).Length <= 10)
					flyTo = extrapolatedTargetPos;
				MoveInDirectionOf(flyTo);
			}
		}

		private void RaiseTargetReached(EnemyBomber plane) {
			if (TargetReached != null)
				TargetReached(this, new TargetReacherEventArgs(plane, target));
		}

		#endregion


		#region Methods

		/// <summary>
		/// Вернуться по окружности к бомбометанию
		/// </summary>
		private void ReturnToTarget() {

		}

		/// <summary>
		/// Не пора ли проводить бомбометания
		/// </summary>
		private bool CanBomb {
			get {
				return World.Instance.Time.TotalTime > targetReachedTime;

				//Vector2D planePos = ControlledDynamicObject.Position.Projection2D;
				//Vector2D targetPos = target.Position.Projection2D;
				//if (MathHelper.Distance(planePos, targetPos) <
				//    ((StaticTargetAI)target.AI).DamageRadius) {
				//    /*мы оказались в зоне поражения*/
				//    return true;
				//}
				//return false;
			}
		}

		/// <summary>
		/// Величина повреждений, наносимых бомбой.
		/// </summary>
		public readonly double BombDamage = 5;
		/// <summary>
		/// Радиус зоны, в которой бомба наносит повреждения.
		/// </summary>
		public readonly double BombDamageRange = Distance.FromMetres(50);

		private void DropBomb() {
			EnemyBomber plane = ControlledBomber;

			/*остались бомбы*/
			if (plane.WeaponsLeft > 0) {
				Vector2D bombPos = plane.Position.Projection2D;

				double plusMinusRandX = rand.NextDouble();
				double plusMinusRandY = rand.NextDouble();
				double plusMinusX = (plusMinusRandX > 0.5) ? 1.0 : -1.0;
				double plusMinusY = (plusMinusRandY > 0.5) ? 1.0 : -1.0;
				double errorX = rand.NextDouble() * errorCoef * plusMinusRandX;
				double errorY = rand.NextDouble() * errorCoef * plusMinusRandY;
				bombPos.X += errorX;
				bombPos.Y += errorY;

				World.Instance.ExplodeBomb(bombPos, BombDamage, BombDamageRange);

				if (target.Health > 0 && plane.WeaponsLeft > 0) {
					returnToTargetCircleCenter = target.Position + plane.Orientation *
						returnToTargetRadius;
					Mode = BomberFlightMode.ReturnToBombTarget;
				}
			}
		}

		private EnemyBomber ControlledBomber {
			get { return (EnemyBomber)ControlledDynamicObject; }
		}

		/// <summary>
		/// Лететь к цели с учетом зоны рлс и маневра при входе в нее
		/// </summary>
		private void FlyToTarget() {
			EnemyBomber plane = ControlledBomber;
			RLS rls = plane.World.SelectAll<RLS>().First();
			double rlsRadius = rls.CoverageRadius;
			if (willManeuver) {
				/*Если нам еще предстоит определить, попали ли мы в зону рлс*/
				Vector2D plane2DPosition = plane.Position.Projection2D;
				Vector2D rls2DPosition = rls.Position.Projection2D;
				double distance = Math.Sqrt((plane2DPosition.X - rls2DPosition.X) *
					(plane2DPosition.X - rls2DPosition.X) + (plane2DPosition.Y - rls2DPosition.Y) *
					(plane2DPosition.Y - rls2DPosition.Y));
				if (distance < rlsRadius) {
					/*вошли в зону рлс*/
					Debug.WriteLine("Bomber in RLS zone");
					willManeuver = false;
					maneuverTick = rand.Next(minManeuverTick, maxManeuverTick);
				}
			}
			else {
				/*То есть уже определили, что мы в зоне рлс*/
				if (maneuverTick == 0) {
					/*Подошло время самого маневра*/
					double r = rand.NextDouble();
					double initH = plane.Position.H;

					Vector3D newPos = new Vector3D(plane.Position.X, plane.Position.Y,
						minHeight + r * (maxHeight - minHeight));

					MoveInDirectionOf(newPos);
					Debug.WriteLine("Bomber maneuvring from height " + initH + " to " + plane.Position.H);

				}
				else
					maneuverTick--;
			}
		}


		private TimeSpan targetReachedTime;
		/// <summary>
		/// Указать цель для бомбардирования. Возвращает false, если самолет летит на базу
		/// </summary>
		/// <param name="target">статичный объект на земле</param>
		/// <returns>Удалось ли установить цель для бомбардирования</returns>
		public bool AttackTarget(StaticTarget target) {
			if (mode != BomberFlightMode.ReturnToBase) {
				this.target = target;
				targetReachedTime = World.Instance.Time.TotalTime + TimeSpan.FromSeconds(target.Position.LengthTo(ControlledBomber.Position) / ControlledBomber.Speed);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase() { }

		/// <summary>
		/// Может ли бомбер лететь дальше бомбардировать цель
		/// </summary>
		/// <param name="warTime"></param>
		/// <returns></returns>
		private bool CanContinue(WarTime warTime) {
			EnemyBomber plane = (EnemyBomber)ControlledDynamicObject;
			if (plane.Orientation != null && plane.Orientation.X != 0 && plane.Orientation.Y != 0 &&
				plane.Orientation.H != 0) {
				//насколько мы улетим по направлению к цели, если продолжим двигаться к ней
				Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
					* plane.Speed;
				/*если после этого мы не сможем вернутся домой, то возвращаемся обратно
				Другое условие возвращения - кончились ракеты
				 */
				if (MathHelper.Distance(plane.Position + shift, basePosition) > plane.FuelLeft ||
					plane.WeaponsLeft <= 0) {
					Mode = BomberFlightMode.ReturnToBase;
					//target = basePosition;
					return false;
				}
			}
			return true;
		}

		public void a() {

		}
		#endregion

	}
}
