using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Collections.Specialized;
using WarLab.SampleUI.WarObjects;
using WarLab.SampleUI.Charts;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Threading;
using System.Threading;
using System.Windows.Media.Animation;
using System.Diagnostics;
using WarLab.SampleUI.AI;
using WarLab.WarObjects;
using WarLab.AI;

namespace WarLab.SampleUI {

	public delegate GraphicalObject RendererCreator(WarObject warObj);

	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class MainWindow : WarWindow {
		protected override void OnLoadedCore() {
			//World.RegisterAIForWarObject<SamplePlaneAI, SamplePlane>();
			World.RegisterAIForWarObject<PathDrivenAI, SamplePlane>();
			//World.RegisterAIForWarObject<SamplePlaneAI, SampleEnemyPlane>();
			World.RegisterAIForWarObject<PathDrivenAI, SampleEnemyPlane>();
			World.RegisterAIForWarObject<ZRKAI, SimpleZRK>();

			SampleEnemyPlane plane = null;
			for (int i = 0; i < 3; i++) {
				double x = StaticRandom.NextDouble() * 500 + 250;
				double y = StaticRandom.NextDouble() * 500 + 250;
				World.AddWarObject(plane = new SampleEnemyPlane(), new Vector3D(x, y, 1));
			}

			Rocket rocket = new Rocket
			{
				TargetPoint = new Vector3D(1000, 1000, 0),
				TimeOfExposion = TimeSpan.FromSeconds(8),
				Speed = Speed.FromKilometresPerHour(200),
				DamageRange = Distance.FromMetres(200),
				Damage = 10
			};

			//World.AddWarObject(rocket, new Vector3D());
			//((ImprovedRocketAI)rocket.AI).Target = plane;

			World.AddWarObject(new RLS(), new Vector3D());
			World.AddWarObject(new SimpleZRK(10), new Vector3D(30, 100));
		}
	}
}
