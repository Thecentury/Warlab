using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab;
using WarLab.WarObjects;
using System.Diagnostics;
using VisualListener;
using WarLab.Path;

namespace EnemyPlanes {
	public delegate void TargetDestroyedHandler(object sender, TargetDestroyedEventArgs args);

	public class EnemyBomberAI : EnemyPlaneAI {
		#region vars
		/// <summary>
		/// Режим полета бомбера
		/// </summary>
		private BomberFlightMode mode;

		/// <summary>
		/// Бомбардируемая цель
		/// </summary>
		private OurStaticObject target;


		public event TargetDestroyedHandler TargetDestroyed;
		/// <summary>
		/// Нижняя высота, на которую можно совершить маневр
		/// </summary>
		public double minHeight = 1000;
		/// <summary>
		/// Верхняя высота, на которую можно совершить маневр
		/// </summary>
		public double maxHeight = 3000;
		/// <summary>
		/// Надо ли делать маневр
		/// </summary>
		private bool willManeuver = true;
		/// <summary>
		/// Через сколько тактов делать маневр после входа в зону рлс
		/// </summary>
		private int maneuverTick = 0;
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
		private Random rand = StaticRandom.Random;
		/// <summary>
		/// Коэффициент, умножаемый на случайное число от 0 до 1. Полученное число определяет
		/// отклонение от цели, в метрах.
		/// </summary> 
		public double errorCoef = 5.0;
		/// <summary>
		/// Радиус окружности, по которой надо пролетететь бомбардировщику, чтоб вернуться
		/// к бомбометанию
		/// </summary>
		public double returnToTargetRadius = 100.0;

		/// <summary>
		/// Центр окружности, по которой надо пролететь бомберу, чтоб вернуться
		/// к бомбометанию
		/// </summary>
		private Vector3D returnToTargetCircleCenter;
		#endregion

		/// <summary>
		/// Создать интеллект вражеского бомбера
		/// </summary>
		public EnemyBomberAI() {
			Mode = BomberFlightMode.MoveToBombTarget;
		}

		#region properties


		/// <summary>
		/// Радиус окружности, в которой летят истребители рядом с бомберов
		/// </summary>
		private double figtersRadius = Distance.FromMetres(100);
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
			private set {
				mode = value;
				if (value == BomberFlightMode.ReturnToBase) {
					TimeSpan toBaseDuration = TimeSpan.FromSeconds(ControlledBomber.Airport.Position.DistanceTo(ControlledBomber.Position) / ControlledBomber.Speed);
					returnToBaseTime = toBaseDuration + World.Instance.Time.TotalTime;

					// восстанавливаем возможность совершать маневр
					this.duringManeuver = false;
					this.maneuverCompleted = false;
					this.maneuverTick = 0;
					this.maneuverDuration = Default.EnemyBomberManeuverDuration;
					this.willManeuver = true;

				}
				else if (value == BomberFlightMode.ReturnToBombTarget) {
					targetReachedTime.Add(TimeSpan.FromSeconds(path.TotalLength / ControlledBomber.Speed));
				}
			}
		}

		/// <summary>
		/// Возвращает бомбардируемую цель
		/// </summary>
		public StaticObject Target {
			get { return target; }
		}

		#endregion

		#region EnemyPlaneAI Implementation

		private Vector3D flyTo;
		public override void Update(WarTime time) {
			EnemyBomber plane = (EnemyBomber)ControlledDynamicObject;
			double currHeight = plane.Position.H;

			if (target != null) {
				flyTo = new Vector3D(target.Position, currHeight);

				if (mode != BomberFlightMode.ReturnToBase) {
					bool canContinue = CanContinueFlyToTarget(time);
					if (!canContinue) {
						landingAim = ReturnToBaseAim.ReloadOrRefuel;
						Mode = BomberFlightMode.ReturnToBase;
					}
				}

				Vector3D newDirection;
				switch (mode) {
					case BomberFlightMode.ReturnToBase:
						newDirection = AirportPosition;
						flyTo = new Vector3D(newDirection, currHeight);
						ReturnToBase(time);
						break;
					case BomberFlightMode.MoveToBombTarget:
						FlyToBombTarget();
						if (ShouldBomb(time)) {
							DropBomb();
						}
						break;
					case BomberFlightMode.ReturnToBombTarget:
						newDirection = ReturnToBombTarget(time);
						flyTo = new Vector3D(newDirection, currHeight);
						break;
					default:
						break;
				}

				MoveInDirectionOf(flyTo);
			}
		}

		/// <summary>
		/// Нужно для совершения разворота при бомбометании
		/// </summary>
		WarPath path = null;

		/// <summary>
		/// Вернуться по окружности к бомбометанию
		/// </summary>
		private Vector3D ReturnToBombTarget(WarTime time) {
			if (!path.IsFinished) {
				Position newPos = path.GetPosition(time.ElapsedTime.TotalSeconds * ControlledDynamicObject.Speed);
				return newPos.Point;
			}
			else {
				mode = BomberFlightMode.MoveToBombTarget;
				return target.Position;
			}
		}

		private void RaiseTargetDestroyed() {
			if (TargetDestroyed != null) {
				TargetDestroyed(this, new TargetDestroyedEventArgs(target));
			}
		}

		#endregion

		#region Methods

		private readonly TimeSpan bombDelayTime = TimeSpan.FromSeconds(4);
		private TimeSpan bombDelay;
		/// <summary>
		/// Не пора ли проводить бомбометания
		/// </summary>
		private bool ShouldBomb(WarTime time) {
			Vector2D planePos = ControlledDynamicObject.Position.Projection2D;
			Vector2D targetPos = target.Position.Projection2D;

			if (bombDelay > TimeSpan.Zero) bombDelay -= time.ElapsedTime;
			if (bombDelay < TimeSpan.Zero) bombDelay = TimeSpan.Zero;

			return //(World.Instance.Time.TotalTime > targetReachedTime) &&
				bombDelay <= TimeSpan.Zero &&
				(planePos.DistanceTo(targetPos) < BombDamageRange);
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

			// остались бомбы
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
				Debug.WriteLine("Бомба сброшена");
				bombDelay = bombDelayTime;
				plane.WeaponsLeft--;


				if (target.Health > 0 && plane.WeaponsLeft > 0) {
					double a, b;

					/*нужен вектор, направленный под углом в 90 градусов вправо (поэтому a*a+
					 b*b=1*1 - просто делаем такое допущение). 
					 * По направлению этого вектора отложим отрезок длины радиуса,
					 и там будет центр окружности, по которой надо пролететь, чтоб вернуться
					 к бомбометанию. Берем скалярное произведение векторов, где
					 первый - вектор позиции самолета (x,y), второй - вектор (a,b).
					 x*a+y*b=0. Отсюда a=-yb/x; b(1-y/x)=1*/

					b = 1 / (1 - ControlledBomber.Orientation.Y / ControlledBomber.Orientation.X);
					a = -ControlledBomber.Orientation.Y * b / ControlledBomber.Orientation.X;

					/*теперь создадим на основе a и b вектор, у которого высота - 0,
					 * потому что это вектор направления. Потом его нормализуем*/

					Vector3D centerVect = new Vector3D(a, b, 0).Normalize();
					returnToTargetCircleCenter = target.Position + centerVect *
						returnToTargetRadius;

					path = new WarPath(new ArcSegment(returnToTargetCircleCenter,
						returnToTargetRadius, CircleOrientation.CCW, ControlledBomber.Position, 360));
					Mode = BomberFlightMode.ReturnToBombTarget;
				}
				else if (target.Health <= 0) {
					/*Обнулим путь, по которому надо совершать круговое движение, чтоб вернуться
					 к бомбометанию. Надо его обнулить, потому что в ReturnToTarget()
					 стоит проверка if (path!=null), и создается новый путь для
					 возвращения к цели*/
					path = null;
					RaiseTargetDestroyed();
				}
				else if (plane.WeaponsLeft < 1) {
					Mode = BomberFlightMode.ReturnToBase;
					landingAim = ReturnToBaseAim.ReloadOrRefuel;
				}
			}
		}

		private EnemyBomber ControlledBomber {
			get { return (EnemyBomber)ControlledDynamicObject; }
		}

		private TimeSpan maneuverDuration = Default.EnemyBomberManeuverDuration;
		private double maneuverHeight;
		
		private bool duringManeuver = false;
		public bool DuringManeuver {
			get { return duringManeuver; }
		}
		
		private bool maneuverCompleted = false;

		/// <summary>
		/// Лететь к цели с учетом зоны рлс и маневра при входе в нее
		/// </summary>
		private void FlyToBombTarget() {
			EnemyBomber plane = ControlledBomber;
			var rlses = plane.World.SelectAll<RLS>().ToList();

			if (rlses.Count == 0) return;

			foreach (var rls in rlses) {
				double rlsRadius = rls.CoverageRadius;
				if (willManeuver) {

					if (rls.IsInCoverage(plane.Position)) {
						/*вошли в зону рлс*/
						Debug.WriteLine("Бомбардировщик вошел в зону РЛС");
						willManeuver = false;
						maneuverTick = rand.Next(minManeuverTick, maxManeuverTick);

						break;
					}
				}
				else {
					/*То есть уже определили, что мы в зоне рлс*/
					if (!maneuverCompleted && maneuverTick == 0) {
						if (!duringManeuver) {
							duringManeuver = true;
							double r = rand.NextDouble();
							double initH = plane.Position.H;
							double minimalHeight = Math.Max(500, Math.Min(minHeight, initH / 2));

							maneuverHeight = minimalHeight + r * (initH - minimalHeight);
							Debug.WriteLine(ControlledBomber.Name + ": маневр по высоте с " + initH.ToString("F0") + " на " + maneuverHeight.ToString("F0") + " м");
						}
					}
					else {
						maneuverTick--;
					}
				}
			}

			if (duringManeuver && maneuverDuration > TimeSpan.Zero) {
				maneuverDuration -= World.Instance.Time.ElapsedTime;

				Vector3D newPos = target.Position;
				newPos.H = maneuverHeight;

				flyTo = newPos;
			}
			else if (duringManeuver) {
				duringManeuver = false;
				maneuverCompleted = true;
			}
		}


		private TimeSpan targetReachedTime;
		/// <summary>
		/// Указать цель для бомбардирования. Возвращает false, если самолет летит на базу
		/// </summary>
		/// <param name="target">статичный объект на земле</param>
		/// <returns>Удалось ли установить цель для бомбардирования</returns>
		public bool AttackTarget(OurStaticObject target) {
			// todo это правильно?
			if (true || mode != BomberFlightMode.ReturnToBase) {
				this.target = target;
				targetReachedTime = World.Instance.Time.TotalTime + TimeSpan.FromSeconds(target.Position.DistanceTo(ControlledBomber.Position) / ControlledBomber.Speed);
				Mode = BomberFlightMode.MoveToBombTarget;
				return true;
			}
			return false;
		}

		private ReturnToBaseAim landingAim = ReturnToBaseAim.ReloadOrRefuel;
		private TimeSpan returnToBaseTime;
		/// <summary>
		/// Лететь в сторону базы
		/// </summary>
		private void ReturnToBase(WarTime time) {
			if (time.TotalTime > returnToBaseTime) {
				LandPlane();
				RaiseLanded(landingAim);
			}
		}

		public event EventHandler<LandingEventArgs> Landed;
		private void RaiseLanded(ReturnToBaseAim aim) {
			if (Landed != null) {
				Landed(this, new LandingEventArgs(aim));
			}
		}

		/// <summary>
		/// Может ли бомбер лететь дальше бомбардировать цель
		/// </summary>
		/// <param name="warTime"></param>
		/// <returns></returns>
		private bool CanContinueFlyToTarget(WarTime warTime) {
			EnemyBomber plane = (EnemyBomber)ControlledDynamicObject;
			if (plane.Orientation != Vector3D.Zero) {
				//насколько мы улетим по направлению к цели, если продолжим двигаться к ней
				Vector3D shift = plane.Orientation * warTime.ElapsedTime.TotalSeconds
					* plane.Speed;
				/*если после этого мы не сможем вернутся домой, то возвращаемся обратно
				Другое условие возвращения - кончились бомбы
				 */
				if (MathHelper.Distance(plane.Position + shift, AirportPosition) > plane.FuelLeft ||
					plane.WeaponsLeft <= 0) {
					return false;
				}
			}
			return true;
		}

		#endregion

		internal void NoTargetsLeft() {
			Mode = BomberFlightMode.ReturnToBase;
			landingAim = ReturnToBaseAim.NoTargets;
		}

		public bool IsTargettedOnMe(OurFighter fighter) {
			OurFighterAI ai = fighter.AI.Cast<OurFighterAI>();
			EnemyBomber bomber = ControlledBomber;

			var convoyingFighters = World.Instance.SelectAll<EnemyHeadquaters>().Select(hq => hq.TargettedPlanes(bomber));
			return ai.TargetPlane == bomber || convoyingFighters.Select(f => ai.TargetPlane == f).Any();
		}
	}
}
