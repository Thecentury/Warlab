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

		[Renders(typeof(SampleEnemyPlane))]
		[Renders(typeof(SamplePlane))]
		[Renders(typeof(OurFighter))]
		private static GraphicalObject CreateForSamplePlane(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png")
			};
		}

		[Renders(typeof(RLS))]
		private static GraphicalObject CreateForRLS(WarObject warObj) {
			return new RLSGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Radar.png")
			};
		}

		[Renders(typeof(ZRK))]
		private static GraphicalObject CreateForZRK(WarObject warObj) {
			return new ZRKGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyBuilding.png"),
				SmallSprite = false
			};
		}

		[Renders(typeof(EnemyFighter))]
		private static GraphicalObject CreateForEnemyPlane(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyPlane.png")
			};
		}
		
		[Renders(typeof(EnemyBomber))]
		private static GraphicalObject CreateForEnemyBomber(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyBomber.png")
			};
		}

		[Renders(typeof(StaticObject))]
		private static GraphicalObject CreateForStaticObject(WarObject warObj) {
			return new StaticObjectGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyBuilding.png")
			};
		}

		[Renders(typeof(OurAirport))]
		private static GraphicalObject CreateForOurAirportObject(WarObject warObj) {
			return new StaticObjectGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\OurAirport.png")
			};
		}

		[Renders(typeof(EnemyAirport))]
		private static GraphicalObject CreateForEnemyAirportObject(WarObject warObj) {
			return new StaticObjectGraph
			{
				StaticObject = (StaticObject)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\EnemyAirport.png")
			};
		}

		//[Renders(typeof(Rocket))]
		private static GraphicalObject CreateForRocket(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Rocket.png")
			};
		}

		[Renders(typeof(Rocket))]
		private static GraphicalObject CreateForMissile(WarObject warObj) {
			return new MissileGraph
			{
				SpriteSource = (Rocket)warObj,
			};
		}

		public static GraphicalObject CreateGraphForWarObject(WarObject warObject) {
			Type type = warObject.GetType();
			if (renderers.ContainsKey(type)) {
				RendererCreator creator = renderers[type];
				return creator(warObject);
			}
			else {
				var keyType = renderers.Keys.Where(t => t.IsAssignableFrom(type)).FirstOrDefault();
				if (keyType != null) {
					return renderers[keyType](warObject);
				}
				else {
					Debug.WriteLine(String.Format("Объекту типа {0} назначен DefaultGraph.", type.Name));
					return new DefaultGraph { WarObject = warObject };
				}
			}
		}
	}
}
