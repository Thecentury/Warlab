using System.Windows.Controls;
using ScientificStudio.Charting.Layers;

namespace ScientificStudio.Charting {
	/// <summary>
	/// Interaction logic for ChartControl.xaml
	/// </summary>
	public partial class ChartControl : UserControl {
		public ChartControl() {
			InitializeComponent();
		}

		public ChartPlotter Plotter {
			get { return plotter; }
		}

		public LayerControl LayerControl {
			get { return layers; }
		}
	}
}
