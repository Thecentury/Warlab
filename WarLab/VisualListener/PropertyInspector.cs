using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace VisualListener {
	public class PropertyInspector : UserControl {
		private PropertyInspector() {
			Loaded += PropertyInspector_Loaded;
		}

		private static PropertyInspector instance = new PropertyInspector();
		public static PropertyInspector Instance {
			get { return instance; }
		}

		public void AddValue<T>(string descr, T value) {
			string str = value.ToString();

			if (values.ContainsKey(descr)) {
				UIInfo uiInfo = values[descr];
				uiInfo.UIElement.Update(str);
				uiInfo.Value = str;
			}
			else {
				PropertyView view = new PropertyView(descr, str);
				UIInfo info = new UIInfo { Value = str, UIElement = view };
				values.Add(descr, info);
				stackPanel.Children.Add(view);
			}
		}

		private readonly Dictionary<string, UIInfo> values = new Dictionary<string, UIInfo>();

		private StackPanel stackPanel;
		private void PropertyInspector_Loaded(object sender, RoutedEventArgs e) {
			stackPanel = new StackPanel { Background = new SolidColorBrush(Color.FromArgb(100, 100, 50, 180)) };
			Content = stackPanel;
		}
	}
}
