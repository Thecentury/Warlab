using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.WarObjects;

namespace WarLab.AI {
	public abstract class FighterAI<TTarget> : PlaneAI where TTarget : Plane {
		private TTarget targetPlane;
		public TTarget TargetPlane {
			get { return targetPlane; }
			protected set {
				if (value == null)
					throw new ArgumentNullException("value");

				targetPlane = value;
			}
		}

		private ReturnToBaseAim aim;
		public ReturnToBaseAim Aim {
			get { return aim; }
			set { aim = value; }
		}

		protected Vector3D Position {
			get { return ControlledPlane.Position; }
		}

		protected TimeSpan returnToBaseTime;
		protected void SetReturnToBaseTime() {
			returnToBaseTime = World.Instance.Time.TotalTime +
				TimeSpan.FromSeconds(AirportPosition.Distance2D(ControlledPlane.Position) / ControlledPlane.Speed);
		}

		protected readonly double LandDistance = Distance.FromMetres(50);
		protected bool ShouldLand(WarTime time) {
			return returnToBaseTime < time.TotalTime && AirportPosition.Distance2D(ControlledPlane.Position) < LandDistance;
		}

		protected readonly TimeSpan rocketLaunchDelayValue = TimeSpan.FromSeconds(5);
		protected TimeSpan rocketLaunchDelay;

		protected void LaunchRocket() {
			double distance = targetPlane.Position.DistanceTo(Position);
			TimeSpan durationOfFlight = TimeSpan.FromSeconds(2 * distance / rocketSpeed);

			TimeSpan timeToExplode = durationOfFlight + World.Instance.Time.TotalTime;

			Vector2D toTarget = (targetPlane.Position - ControlledPlane.Position).Projection2D.Normalize();
			// не стрелять в заднюю полусферу - только по направлению полета
			if ((ControlledPlane.Orientation.Projection2D & toTarget) > 0.1) {

				TimeSpan realDurationOfFlight = TimeSpan.FromSeconds(distance / rocketSpeed);

				Vector3D extrapolatedPos = targetPlane.Position + targetPlane.Speed * targetPlane.Orientation * realDurationOfFlight.TotalSeconds;
				Rocket rocket = new Rocket
				{
					Speed = rocketSpeed,
					TimeOfExposion = timeToExplode,
					TargetPoint = extrapolatedPos, //targetPlane.Position,
					Host = RocketHost.Plane,
					Side = GetSide()
				};

				World.Instance.AddObject(rocket, Position);

				rocketLaunchDelay = rocketLaunchDelayValue;

				ControlledPlane.WeaponsLeft--;
				if (ControlledPlane.WeaponsLeft <= 0) {
					BeginReturnToBase();
					aim = ReturnToBaseAim.ReloadOrRefuel;
				}
			}
		}

		protected abstract Side GetSide();

		protected abstract void BeginReturnToBase();

		protected readonly double attackDistance = Distance.FromKilometres(0.5);
		protected Vector3D FollowTarget(WarTime time) {
			if (rocketLaunchDelay > TimeSpan.Zero)
				rocketLaunchDelay -= time.ElapsedTime;

			if (rocketLaunchDelay < TimeSpan.Zero)
				rocketLaunchDelay = TimeSpan.Zero;

			double distanceToTarget = TargetPlane.Position.Distance2D(Position);
			if (distanceToTarget < attackDistance
				&& rocketLaunchDelay == TimeSpan.Zero) {
				LaunchRocket();
			}

			double speed = ControlledPlane.Speed;
			if (distanceToTarget < attackDistance / 2) {
				speed = TargetPlane.Speed;
			}
			else if (distanceToTarget < attackDistance) {
				speed = TargetPlane.Speed * 1.15;
			}
			else {
				speed = TargetPlane.Speed * 1.3;
			}
			
			if (speed > 2 * Default.FighterSpeed) {
				speed = 2 * Default.FighterSpeed;
			}

			ControlledPlane.Speed = speed;

			Vector3D position = TargetPlane.Position;
			return position;
		}

		private double rocketSpeed = Speed.FromKilometresPerHour(300);
		public double RocketSpeed {
			get { return rocketSpeed; }
			set {
				Verify.IsPositive(value);

				rocketSpeed = value;
			}
		}
	}
}
