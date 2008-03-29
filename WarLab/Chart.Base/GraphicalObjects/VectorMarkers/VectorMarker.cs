using System;
using System.Windows;
using System.Windows.Media;
using ScientificStudio.Charting.Isoline;

namespace ScientificStudio.Charting.GraphicalObjects.VectorMarkers {
	public abstract class VectorMarker {
#if DEBUG
		bool inited = false;
#endif
		public virtual void Init(VectorField2d field) {
#if DEBUG
			inited = true;
#endif
		}

		private Size markerSize;
		public Size MarkerSize {
			get { return markerSize; }
		}

#if DEBUG
		bool preRenderInited = false;
#endif
		public virtual void PreRenderInit(Size markerSize) {
#if DEBUG
			preRenderInited = true;
#endif
			this.markerSize = markerSize;
		}

		public virtual void Render(DrawingContext dc, Point pos, Vector2D dir) {
#if DEBUG
			if (!inited || !preRenderInited) {
				throw new InvalidOperationException("Uninitialized vector marker!");
			}
#endif
		}
	}
}
