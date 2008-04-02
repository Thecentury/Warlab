using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects {
	/// <summary>
	/// Target of rendering
	/// </summary>
	public enum RenderTo {
		/// <summary>
		/// Rendering directly to screen
		/// </summary>
		Screen,
		/// <summary>
		/// Rendering to bitmap, which will be drawn to screen later.
		/// </summary>
		Image
	}

	public interface IAbortable {
		void BeginAbort();
		bool AbortPending { get; }
	}

	public sealed class RenderState : IAbortable {
		public readonly Rect Visible;
		public readonly Rect Output;
		public readonly Rect OutputWithMargin;
		public readonly Rect RenderVisible;
		public readonly RenderTo RenderingType;

		void IAbortable.BeginAbort() {
			abortPending = true;
		}

		private bool abortPending = false;
		public bool AbortPending {
			get { return abortPending; }
		}

		public RenderState(Rect renderVisible, Rect visible, Rect output, Rect outputWithMargin, RenderTo renderingType) {
			this.RenderVisible = renderVisible;
			this.Visible = visible;
			this.Output = output;
			this.OutputWithMargin = outputWithMargin;
			this.RenderingType = renderingType;
		}
	}
}
