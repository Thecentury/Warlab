using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EnemyPlanes;
using WarLab.WarObjects;
using System.Diagnostics;

namespace WarLab.SampleUI {
	/// <summary>
	/// Interaction logic for Window2.xaml
	/// </summary>
	public partial class Window2 : WarWindow {
		public Window2() {
			InitializeComponent();
		}

		protected override void OnLoadedCore() {
			World.RegisterAIForWarObject<EnemyFighterAI, EnemyFighter>();
			World.RegisterAIForWarObject<EnemyBomberAI, EnemyBomber>();
			World.RegisterAIForWarObject<StaticTargetAI, StaticTarget>();

			double fuel = Distance.FromKilometres(3);

			EnemyAirport bomberAirport = new EnemyAirport { PlaneLaunchDelay = TimeSpan.FromSeconds(10) };
			bomberAirport.AddPlanes((i) => new EnemyBomber { WeaponsCapacity = 4 }, 10);
			bomberAirport.AddPlanes((i) => new EnemyFighter(), 300);

			EnemyAirport fighterAirport = new EnemyAirport();
			//fighterAirport.AddPlanes(i => new EnemyFighter(), 15);
			//fighterAirport.AddPlanes(i => new EnemyBomber { WeaponsCapacity = 3 }, 5);

			World.AddObject(bomberAirport, new Vector3D(-1400, 200, 0));
			World.AddObject(fighterAirport, new Vector3D(-1200, 1000, 0));

			World.AddObject(new EnemyHeadquaters(), new Vector3D(0, 0));

			World.AddObject(new StaticTarget { Health = 20, Importance = 0.1 }, new Vector3D(1650, 200));

			World.AddObject(new RLS { Health = 10, CoverageRadius = 1000, Importance = 0 }, new Vector3D(800, 250));
			//World.AddObject(new RLS { Health = 10, CoverageRadius = 500 }, new Vector3D(500, -400));

			//World.AddObject(new ZRK { NumOfChannels = 1, CoverageRadius = 300, NumOfEquipment = 200, Health = 10 }, new Vector3D(500, -350));
			World.AddObject(new ZRK { NumOfChannels = 4, CoverageRadius = 300, NumOfEquipment = 200, Health = 10, Importance = 20 }, new Vector3D(450, 550));
			World.AddObject(new SingleChannelZRK { CoverageRadius = 200, NumOfEquipment = 100, Health = 5, Importance = 20 }, new Vector3D(450, 0));
			
			World.AddObject(new OurHeadquaters(), new Vector3D(800, 250));
			OurAirport ourAirport = new OurAirport { Importance = 200 };
			ourAirport.AddPlanes((i) => new OurFighter(), 4);
			World.AddObject(ourAirport, new Vector3D(1000, 300));
		}
	}
}
