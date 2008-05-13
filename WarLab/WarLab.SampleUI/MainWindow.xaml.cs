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
using EnemyPlanes;

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
			World.RegisterAIForWarObject<ZRKAI, ZRK>();

			SampleEnemyPlane plane = null;
			for (int i = 0; i < 3; i++) {
				double x = StaticRandom.NextDouble() * 500 + 250;
				double y = StaticRandom.NextDouble() * 500 + 250;
				World.AddObject(plane = new SampleEnemyPlane(), new Vector3D(x, y, 1));
			}

			Rocket rocket = new Rocket
			{
				TargetPosition = new Vector3D(1000, 1000, 0),
				TimeOfExposion = TimeSpan.FromSeconds(8),
				Speed = Speed.FromKilometresPerHour(200),
				DamageRange = Distance.FromMetres(200),
				Damage = 10
			};

			//World.AddWarObject(rocket, new Vector3D());
			//((ImprovedRocketAI)rocket.AI).Target = plane;

			World.AddObject(new RLS(), new Vector3D());
			World.AddObject(new ZRK { NumOfChannels = 10 }, new Vector3D(30, 100));
			World.AddObject(new EnemyHeadquaters(), new Vector3D(500, 500));
		}
	}
}
