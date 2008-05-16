using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace VisualListener {
	public class PropertyInspectorControl : UserControl {
		public PropertyInspectorControl() {
			PropertyInspector.AttachControl(this);
			
			stackPanel = new StackPanel { Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0)) };
			Content = stackPanel;
		}

		public void AddValue<T>(string descriptiveKey, T value) {
			string str = value.ToString();

			if (values.ContainsKey(descriptiveKey)) {
				UIInfo uiInfo = values[descriptiveKey];
				if (uiInfo.Value != str) {
					uiInfo.UIElement.Update(str);
					uiInfo.Value = str;
				}
			}
			else {
				PropertyView view = new PropertyView(descriptiveKey, str);
				UIInfo info = new UIInfo { Value = str, UIElement = view };
				values.Add(descriptiveKey, info);
				stackPanel.Children.Add(view);
			}
		}

		private readonly Dictionary<string, UIInfo> values = new Dictionary<string, UIInfo>();

		private readonly StackPanel stackPanel;
	}
}
