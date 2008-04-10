using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace WarLab.SampleUI {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
		protected override void OnStartup(StartupEventArgs e) {
			base.OnStartup(e);

			string windowName = "";
#if true
			windowName = "MainWindow";
#elif true
			windowName = "MainWindow2";
#endif
			windowName += ".xaml";
			StartupUri = new Uri(windowName, UriKind.Relative);
		}
	}
}
