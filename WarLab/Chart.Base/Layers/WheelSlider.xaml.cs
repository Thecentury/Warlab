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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScientificStudio.Charting.Layers {
	/// <summary>
	/// Slider with mouse wheel events, taken into account.
	/// </summary>
	public partial class WheelSlider : Slider {
		public WheelSlider() {
			InitializeComponent();
		}

		private void Slider_MouseWheel(object sender, MouseWheelEventArgs e) {
			if (e.Delta != 0) {
				double delta = e.Delta / 120.0;
				Value += 5 * delta * LargeChange;
			}
		}
	}
}
