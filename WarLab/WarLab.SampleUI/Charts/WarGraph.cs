using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Controls;
using System.Windows;
using ScientificStudio.Charting;

namespace WarLab.SampleUI.Charts {
	public abstract class WarGraph : GraphicalObject {
		public WarGraph() {
			MenuItem menuItem = new MenuItem { Header = "Show properties" };
			menuItem.Click += menuItem_Click;
			List<MenuItem> menuItems = new List<MenuItem> {
				menuItem
			};
			ContextMenu menu = new ContextMenu();
			menu.ItemsSource = menuItems;
			ContextMenu = menu;
		}

		private void menuItem_Click(object sender, RoutedEventArgs e) {
			PropertyGridWindow propertyWindow = new PropertyGridWindow
			{
				SelectedObject = WarObject
			};
			propertyWindow.Show();
		}

		private WarObject warObject;
		public WarObject WarObject {
			get { return warObject; }
			set { warObject = value; }
		}

		protected ChartPlotter ParentPlotter {
			get { return (ChartPlotter)Parent; }
		}

		public void DoUpdate() {
			if (warObject != null) {
				Panel.SetZIndex(this, (int)warObject.Position.H);
			}
			MakeDirty();
			InvalidateVisual();
		}
	}
}
