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
using System.Windows.Forms;
using System.ComponentModel;

namespace WarLab.SampleUI {
	/// <summary>
	/// Interaction logic for PropertyGridWondow.xaml
	/// </summary>
	public partial class PropertyGridWindow : Window {
		private PropertyGrid propertyGrid;
		public PropertyGridWindow() {
			InitializeComponent();

			propertyGrid = new PropertyGrid();
			host.Child = propertyGrid;
		}

		public object SelectedObject {
			get { return propertyGrid.SelectedObject; }
			set {
				Title = "Properties of " + value;
				propertyGrid.SelectedObject = value;
			}
		}
	}
}
