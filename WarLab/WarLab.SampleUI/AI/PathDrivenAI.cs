using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab.AI;
using WarLab.SampleUI.WarObjects;
using WarLab.Path;

namespace WarLab.SampleUI.AI {
	[Controls(typeof(SampleEnemyPlane))]
	[Controls(typeof(SamplePlane))]
	class PathDrivenAI : DynamicObjectAI {

		WarPath path = null;
		public override void Update(WarTime time) {
			if (path == null) {
				path = new WarPath (
				new ArcSegment(ControlledDynamicObject.Position, 100, CircleOrientation.CCW, 0, 720),
				new LineSegment
				{
					StartPoint = ControlledDynamicObject.Position,
					EndPoint = new Vector3D(0, 0, 0)
				},
				new LineSegment
				{
					StartPoint = new Vector3D(0, 0, 0),
					EndPoint = new Vector3D(950, 0, 0)
				},
				new ArcSegment(new Vector3D(950, 50, 0), 50, CircleOrientation.CCW, -90, 0),
				new LineSegment
				{
					StartPoint = new Vector3D(1000, 50, 0),
					EndPoint = new Vector3D(1000, 1000, 0)
				});
			}

			if (!path.IsFinished) {
				Position newPos = path.GetPosition(time.ElapsedTime.TotalSeconds * ControlledDynamicObject.Speed);
				MoveInDirectionOf(newPos.Point);
			}
		}
	}
}
