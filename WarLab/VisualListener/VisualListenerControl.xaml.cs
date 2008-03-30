using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace VisualListener {
	/// <summary>
	/// Interaction logic for VisualListenerControl.xaml
	/// </summary>
	public partial class VisualListenerControl : UserControl {
		private readonly VisualListener listener;

		public TraceListener Listener {
			get { return listener; }
		}

		public VisualListenerControl() {
			InitializeComponent();

			// don't add self to listeners if in design mode
			if (!DesignerProperties.GetIsInDesignMode(this)) {
				listener = new VisualListener(this);
				Debug.Listeners.Add(listener);
			}
		}

		public void Write(string message) {
			WriteLine(message);
		}

		public void WriteLine(string message) {
			Dispatcher.BeginInvoke(
				DispatcherPriority.Send,
				(LogWrite)
				delegate() {
					AddMessage(new TextBlock(new Run(message)));
				});
		}

		private Color defaultColor = Colors.Black;
		public Color DefaultColor {
			get { return defaultColor; }
			set { defaultColor = value; }
		}

		private delegate void LogWrite();
		public void WriteLine(string message, string category) {
			Dispatcher.BeginInvoke(
				DispatcherPriority.Send,
				(LogWrite)
				delegate() {
					if (category == null) throw new ArgumentNullException("category");

					Color c = defaultColor;

					string innerMessage;

					// try to determine Color by its name
					Type colorsType = typeof(Colors);
					PropertyInfo colorProperty = colorsType.GetProperty(category);
					// if there is Color with name "color"
					if (colorProperty != null) {
						c = (Color)colorProperty.GetValue(null, null);
						innerMessage = message;
					}
					else {
						innerMessage = category + ": " + message;
					}

					AddMessage(new TextBlock(new Run { Text = innerMessage, Foreground = new SolidColorBrush(c) }));
				});
		}

		private void AddMessage(UIElement element) {
			stackPanel.Children.Add(element);
			scrollViewer.ScrollToBottom();
		}

		public void ClearLog() {
			stackPanel.Children.Clear();
		}
	}
}
