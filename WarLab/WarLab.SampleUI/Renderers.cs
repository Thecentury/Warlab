using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ScientificStudio.Charting.GraphicalObjects;
using WarLab.SampleUI.WarObjects;
using WarLab.SampleUI.Charts;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using WarLab.WarObjects;
using EnemyPlanes;

namespace WarLab.SampleUI {
	public static class Renderers {
		private static readonly Dictionary<Type, RendererCreator> renderers = new Dictionary<Type, RendererCreator>();

		static Renderers() {
			Type type = typeof(Renderers);

			MemberInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
			var filteredMethods = from meth in methods
								  let attrs = meth.GetCustomAttributes(typeof(RendersAttribute), false)
								  where attrs.Length > 0
								  from attr in attrs
								  select new { Method = meth, WarType = ((RendersAttribute)attr).WarObjectType };

			foreach (var item in filteredMethods) {
				renderers.Add(item.WarType, (RendererCreator)Delegate.CreateDelegate(typeof(RendererCreator), (MethodInfo)item.Method));
			}
		}

		[RendersAttribute(typeof(SampleEnemyPlane))]
		[RendersAttribute(typeof(SamplePlane))]
		private static GraphicalObject CreateForSamplePlane(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png")
			};
		}

		[RendersAttribute(typeof(RLS))]
		private static GraphicalObject CreateForStaticObject(WarObject warObj) {
			return new RLSGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyBuilding.png")
			};
		}

		[RendersAttribute(typeof(EnemyFighter))]
		[RendersAttribute(typeof(EnemyAirport))]
		[RendersAttribute(typeof(EnemyBomber))]
		private static GraphicalObject CreateForBomber(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyPlane.png")
			};
		}

		[RendersAttribute(typeof(StaticTarget))]
		private static GraphicalObject CreateForStaticTarget(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyBuilding.png")
			};
		}
		
		[RendersAttribute(typeof(Rocket))]
		private static GraphicalObject CreateForRocket(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Rocket.png")
			};
		}

		public static GraphicalObject CreateGraphForWarObject(WarObject warObject) {
			RendererCreator creator = renderers[warObject.GetType()];
			return creator(warObject);
		}
	}
}
