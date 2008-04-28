using System.Windows.Controls;
using System.Windows.Input;

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
				Value += 5 * delta * SmallChange;
			}
		}
	}
}
