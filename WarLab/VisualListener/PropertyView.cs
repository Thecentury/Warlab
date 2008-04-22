using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace VisualListener {
	internal sealed class PropertyView : UserControl {
		private StackPanel panel;
		private TextBlock dText;
		private TextBlock vText;

		public PropertyView(string descr, string value) {
			panel = new StackPanel { Orientation = Orientation.Horizontal };
			dText = new TextBlock(new Run(descr));
			vText = new TextBlock(new Run(value));
			vText.Margin = new Thickness(10, 0, 0, 0);
			panel.Children.Add(dText);
			panel.Children.Add(vText);

			Content = panel;
		}

		public void Update(string value) {
			vText.Text = value;
			vText.Foreground = Brushes.Red;
		}
	}
}
