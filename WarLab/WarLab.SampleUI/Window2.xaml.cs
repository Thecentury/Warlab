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

			EnemyAirport bomberAirport = new EnemyAirport();
			bomberAirport.AddPlanes(new EnemyBomber());
			EnemyAirport fighterAirport = new EnemyAirport();
			fighterAirport.AddPlanes(new EnemyFighter());

			enemyManager = new EnemyHeadquaters(bomberAirport, fighterAirport);

			double fuel = Distance.FromKilometres(10000);

			//EnemyBomber bomber = new EnemyBomber(10, fuel, 60);
			//EnemyFighter fighter1 = new EnemyFighter(10, fuel, 140.0);
			//EnemyFighter fighter2 = new EnemyFighter(10, fuel, 120.0);
			//EnemyFighter fighter3 = new EnemyFighter(10, fuel, 120.0);

			World.AddWarObject(bomberAirport, new Vector3D(10, 100, 10));
			World.AddWarObject(fighterAirport, new Vector3D(10, 500, 10));

			//World.AddWarObject(bomber, bomberAirport.Position);
			//World.AddWarObject(fighter1, fighterAirport.Position);
			//World.AddWarObject(fighter2, fighterAirport.Position);
			//World.AddWarObject(fighter3, fighterAirport.Position);

			//fighterAirport.AddPlanes(fighter1);
			//fighterAirport.AddPlanes(fighter2);
			//fighterAirport.AddPlanes(fighter3);

			//((EnemyBomberAI)bomber.AI).FightersRadius = 60.0;
			//((EnemyBomberAI)bomber.AI).TargetReached += MainWindow_targetReached;

			target1 = new StaticTarget();
			target2 = new StaticTarget();
			World.AddWarObject(target1, new Vector3D(650, 250));
			World.AddWarObject(target2, new Vector3D(30, 900));

			//enemyManager.Navigate(bomber, target1);
			//enemyManager.Navigate(fighter1, bomber);
			//enemyManager.Navigate(fighter2, bomber);
			//enemyManager.Navigate(fighter3, bomber);
		}

		EnemyHeadquaters enemyManager;
		StaticTarget target2;
		StaticTarget target1;

		private void MainWindow_targetReached(object sender, TargetReacherEventArgs args) {
			if (args.Target == target1)
				enemyManager.Navigate(args.Bomber, target2);
			else if (args.Target == target2)
				enemyManager.Navigate(args.Bomber, target1);
		}
	}
}
