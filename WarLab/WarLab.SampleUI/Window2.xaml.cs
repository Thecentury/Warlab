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
			World.RegisterAIForWarObject<EnemyAirportAI, EnemyAirport>();
			World.RegisterAIForWarObject<StaticTargetAI, StaticTarget>();

			double fuel = Distance.FromKilometres(3);

			EnemyAirport bomberAirport = new EnemyAirport { PlaneLaunchDelay = TimeSpan.FromSeconds(10) };
			bomberAirport.AddPlanes((i) => new EnemyBomber(3, fuel, 10), 20);
			EnemyAirport fighterAirport = new EnemyAirport();
			fighterAirport.AddPlanes(new EnemyFighter(1, fuel, 10));
			fighterAirport.AddPlanes(i => new EnemyBomber(3, fuel, 10), 10);

			enemyHeadquaters = new EnemyHeadquaters();
			World.AddWarObject(enemyHeadquaters, new Vector3D(0, 0));

			//EnemyBomber bomber = new EnemyBomber(10, fuel, 60);
			//EnemyFighter fighter1 = new EnemyFighter(10, fuel, 140.0);
			//EnemyFighter fighter2 = new EnemyFighter(10, fuel, 120.0);
			//EnemyFighter fighter3 = new EnemyFighter(10, fuel, 120.0);

			World.AddWarObject(bomberAirport, new Vector3D(-400, 100, 0));
			World.AddWarObject(fighterAirport, new Vector3D(-400, 500, 00));

			//World.AddWarObject(bomber, bomberAirport.Position);
			//World.AddWarObject(fighter1, fighterAirport.Position);
			//World.AddWarObject(fighter2, fighterAirport.Position);
			//World.AddWarObject(fighter3, fighterAirport.Position);

			//fighterAirport.AddPlanes(fighter1);
			//fighterAirport.AddPlanes(fighter2);
			//fighterAirport.AddPlanes(fighter3);

			//((EnemyBomberAI)bomber.AI).FightersRadius = 60.0;
			//((EnemyBomberAI)bomber.AI).TargetReached += MainWindow_targetReached;

			target1 = new StaticTarget { Health = 10 };
			target2 = new StaticTarget { Health = 10 };
			World.AddWarObject(target1, new Vector3D(650, 250));
			World.AddWarObject(target2, new Vector3D(30, 900));

			World.AddWarObject(new RLS
			{
				Health = 10,
				CoverageRadius = 500
			}, new Vector3D(500, 500));

			//World.AddWarObject(new SimpleZRK(1), new Vector3D(550, 450)).NumOfEquipment = 200;
			World.AddWarObject(new ZRK(100), new Vector3D(450, 550)).NumOfEquipment = 200000;
			//zrk.NumOfEquipment = 200;


			//enemyManager.Navigate(bomber, target1);
			//enemyManager.Navigate(fighter1, bomber);
			//enemyManager.Navigate(fighter2, bomber);
			//enemyManager.Navigate(fighter3, bomber);
		}

		EnemyHeadquaters enemyHeadquaters;
		StaticTarget target2;
		StaticTarget target1;
	}
}
