using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Windows.Threading;

namespace VisualListener {
	internal sealed class PropertyView : UserControl {
		private StackPanel panel;
		private TextBlock dText;
		private TextBlock vText;
		private Timer timer;

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

			if (timer != null) {
				timer.Change(highlightDuration, highlightDuration);
			}
			else {
				timer = new Timer(OnTimerTick, null, highlightDuration, highlightDuration);
			}
		}

		const int highlightDuration = 3000; // milliseconds

		private void OnTimerTick(object state) {
			Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
			{
				vText.Foreground = Brushes.Black;
				timer.Change(Timeout.Infinite, Timeout.Infinite);
			}));
		}
	}
}
