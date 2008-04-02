using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows;
using System.Windows.Controls;

namespace ScientificStudio.Charting.Layers {
	/// <summary>
	/// Proxy object for GraphicalObject, used in LevelControl.
	/// </summary>
	public sealed class Layer {
		private readonly GraphicalObject graph;
		public GraphicalObject Graph {
			get { return graph; }
		}

		[Obsolete("It is not intended that you create Layer manually", true)]
		public Layer() { }

		public Layer(GraphicalObject graph) {
			if (graph == null)
				throw new ArgumentNullException("graph");

			this.graph = graph;
		}

		public bool Visible {
			get { return graph.Visibility == Visibility.Visible; }
			set { graph.Visibility = value ? Visibility.Visible : Visibility.Hidden; }
		}

		public double Opacity {
			get { return graph.Opacity * 100; }
			set { graph.Opacity = value / 100; }
		}

		public int ZIndex {
			get { return Panel.GetZIndex(graph); }
		}
	}
}
