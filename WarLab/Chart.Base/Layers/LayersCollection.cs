using System;
using System.Linq;
using System.Collections.Generic;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Controls;
using System.Windows;
using ScientificStudio.Charting.Layers;

namespace ScientificStudio.Charting.Layers {
	public sealed class LayersCollection : List<Layer> {
		private LayersCollection(UIElementCollection collection) {
			if (collection == null)
				throw new ArgumentNullException("collection");

			this.collection = collection;
			Populate();
		}

		public void Update() {
			Clear();
			Populate();
		}

		private void Populate() {
			// adding only GraphicalObjects
			foreach (GraphicalObject graph in collection.OfType<GraphicalObject>()) {
				if (graph.IsLayer) {
					Add(new Layer(graph));
				}
			}

			for (int i = 0; i < Count; i++) {
				Panel.SetZIndex(this[i].Graph, i + 1);
			}
		}

		private readonly UIElementCollection collection;
		public UIElementCollection Collection {
			get { return collection; }
		}

		internal static LayersCollection LoadFrom(UIElementCollection collection) {
			LayersCollection levels = new LayersCollection(collection);
			return levels;
		}
	}
}
