﻿using System;
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
			bomberAirport.AddPlanes((i) => new EnemyBomber { WeaponsCapacity = 3 }, 10);
			bomberAirport.AddPlanes((i) => new EnemyFighter(), 10);

			EnemyAirport fighterAirport = new EnemyAirport();
			//fighterAirport.AddPlanes(new EnemyFighter());
			fighterAirport.AddPlanes(i => new EnemyBomber { WeaponsCapacity = 20 }, 1);

			World.AddObject(bomberAirport, new Vector3D(-400, 100, 0));
			World.AddObject(fighterAirport, new Vector3D(-400, 500, 0));

			World.AddObject(new EnemyHeadquaters(), new Vector3D(0, 0));

			target1 = new StaticTarget { Health = 10 };
			target2 = new StaticTarget { Health = 10 };
			World.AddObject(target1, new Vector3D(650, 250));
			World.AddObject(target2, new Vector3D(30, 900));

			World.AddObject(new RLS
			{
				Health = 10,
				CoverageRadius = 500
			}, new Vector3D(500, 500));

			World.AddObject(new ZRK { NumOfChannels = 1, CoverageRadius = 300, NumOfEquipment = 20000, Health = 1000 }, new Vector3D(450, 550));
			World.AddObject(new OurHeadquaters(), new Vector3D(300, 800));
			OurAirport ourAirport = new OurAirport();
			ourAirport.AddPlanes((i) => new OurFighter(), 10);
			World.AddObject(ourAirport, new Vector3D(800, 300));
		}

		StaticTarget target2;
		StaticTarget target1;
	}
}
