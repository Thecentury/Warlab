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
								  select new { Method = meth, WarType = ((RendersAttribute)attrs[0]).WarObjectType };

			foreach (var item in filteredMethods) {
				renderers.Add(item.WarType, (RendererCreator)Delegate.CreateDelegate(typeof(RendererCreator), (MethodInfo)item.Method));
			}
		}

		[RendersAttribute(typeof(SamplePlane))]
		private static GraphicalObject CreateForSamplePlane(WarObject warObj) {
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png") //new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			};
		}

		[RendersAttribute(typeof(EnemyBomber))]
		private static GraphicalObject CreateForBomber(WarObject warObj)
		{
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png") //new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			};
		}

		[RendersAttribute(typeof(EnemyFighter))]
		private static GraphicalObject CreateForFighter(WarObject warObj)
		{
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png") //new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			};
		}


		[RendersAttribute(typeof(EnemyAirport))]
		private static GraphicalObject CreateForFightersAirport(WarObject warObj)
		{
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png") //new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			};
		}

		[RendersAttribute(typeof(StaticTarget))]
		private static GraphicalObject CreateForStaticTarget(WarObject warObj)
		{
			return new SpriteGraph
			{
				SpriteSource = (ISpriteSource)warObj,
				SpriteImage = ResourceManager.GetBitmap(@"Sprites\Plane.png") //new BitmapImage(new Uri(@"Sprites\Plane.png", UriKind.Relative))
			};
		}

		public static GraphicalObject CreateGraphForWarObject(WarObject warObject) {
			RendererCreator creator = renderers[warObject.GetType()];
			return creator(warObject);
		}
	}
}
