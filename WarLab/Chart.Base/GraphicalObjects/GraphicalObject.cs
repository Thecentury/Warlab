#define comp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ScientificStudio.Charting.Auxilliary;
using ScientificStudio.Charting.GraphicalObjects.Descriptions;

namespace ScientificStudio.Charting.GraphicalObjects {
	public abstract class GraphicalObject : FrameworkElement, INotifyPropertyChanged, IGraphicalObject {

		public virtual Rect ContentBounds { get { return Rect.Empty; } }
		public event EventHandler ContentBoundsChanged;
		protected void RaiseContentBoundsChanged() {
			if (ContentBoundsChanged != null) {
				ContentBoundsChanged(this, EventArgs.Empty);
			}
		}

		private Computator computator = null;
		/// <summary>
		/// Gets the computator.
		/// </summary>
		/// <value>The computator.</value>
		private Computator Computator {
			get {
				if (computator == null) {
					computator = new Computator(this);
				}
				return computator;
			}
		}

		#region Viewport

		private Viewport2D viewport = null;
		protected Viewport2D Viewport {
			get { return viewport; }
		}

		void IGraphicalObject.SetViewport(Viewport2D viewport) {
			DetachViewport(this.viewport);
			this.viewport = viewport;
			AttachViewport(viewport);
			OnViewportChanged();
		}

		void IGraphicalObject.DetachViewport() {
			DetachViewport(viewport);
		}

		private void DetachViewport(Viewport2D viewport) {
			if (viewport != null) {
				viewport.RectChanged -= OnViewportRectChanged;
			}
		}

		private void AttachViewport(Viewport2D viewport) {
			if (viewport == null)
				throw new ArgumentNullException("viewport");

			viewport.RectChanged += OnViewportRectChanged;
		}

		protected virtual void OnViewportRectChanged(Viewport2D viewport, DependencyPropertyChangedEventArgs e) {
			if (e.Property == Viewport2D.OutputProperty) {
				OnOutputChanged((Rect)e.NewValue, (Rect)e.OldValue);
			}
			else if (e.Property == Viewport2D.VisibleProperty) {
				OnVisibleChanged((Rect)e.NewValue, (Rect)e.OldValue);
			}
			else {
				// other rects changed are now not interesting for us
			}
			InvalidateVisual();
		}

		#endregion

		protected void MakeDirty() {
			IsDirty = true;
			shift = new Vector();
			ClearCache();
		}

		#region Description

		/// <summary>
		/// Creates the default description.
		/// </summary>
		/// <returns></returns>
		protected virtual Description CreateDefaultDescription() {
			return new StandartDescription();
		}

		private Description description = null;
		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		/// <value>The description.</value>
		public Description Description {
			get {
				if (description == null) {
					description = CreateDefaultDescription();
					description.Attach(this);
				}
				return description;
			}
			set {
				if (description != null) {
					description.Detach();
				}
				description = value;
				if (description != null) {
					description.Attach(this);
				}
				RaisePropertyChanged("Description");
			}
		}

		public override string ToString() {
			return Description.Brief;
		}

		#endregion

		protected bool IsDirty = false;

		protected Vector shift = new Vector();

		/// <summary>
		/// Called when [visible changed].
		/// </summary>
		/// <param name="newRect">The new rect.</param>
		/// <param name="oldRect">The old rect.</param>
		protected virtual void OnVisibleChanged(Rect newRect, Rect oldRect) {
			if (newRect.Size == oldRect.Size) {
				IsDirty = false;
				Rect output = Viewport.OutputWithMargin;
				shift += oldRect.Location.Transform(Viewport.Visible, output) - newRect.Location.Transform(Viewport.Visible, output);
			}
			else {
				MakeDirty();
			}
		}

		protected virtual void OnOutputChanged(Rect newRect, Rect oldRect) {
			MakeDirty();
		}

		/// <summary>
		/// Gets a value indicating whether this instance is translated.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is translated; otherwise, <c>false</c>.
		/// </value>
		protected bool IsTranslated {
			get { return shift.X != 0 || shift.Y != 0; }
		}

		protected virtual void OnViewportChanged() { }

		protected Point ZeroOnScreen {
			get { return new Point(0, 0).Transform(Viewport.Visible, Viewport.OutputWithMargin); }
		}

		#region IsLevel

		public bool IsLayer {
			get { return (bool)GetValue(IsLayerProperty); }
			set { SetValue(IsLayerProperty, value); }
		}

		public static readonly DependencyProperty IsLayerProperty =
			DependencyProperty.Register(
			"IsLayer",
			typeof(bool),
			typeof(GraphicalObject),
			new FrameworkPropertyMetadata(
				false
				));

		#endregion

		#region Rendering & caching options

		private void ClearCache() {
			if (renderTarget == RenderTo.Image) {
#if !comp
				foreach (ImageComputionOperation comp in operations) {
					comp.Operation.Abort();
				}
#else
				Computator.AbortAllOperations();
#endif
				imageCache.Clear();

				GC.Collect(2, GCCollectionMode.Optimized);
				GC.WaitForPendingFinalizers();
				GC.Collect(2, GCCollectionMode.Optimized);
			}
		}

		protected object GetValueSync(DependencyProperty p) {
			try {
				return Dispatcher.Invoke(
							   DispatcherPriority.Send,
							   (DispatcherOperationCallback)delegate { return GetValue(p); },
							   p);
			}
			catch (Exception) {
				return p.DefaultMetadata.DefaultValue;
			}
		}

		protected void SetValueAsync(DependencyProperty p, object value) {
			Dispatcher.BeginInvoke(DispatcherPriority.Send,
				(SendOrPostCallback)delegate { SetValue(p, value); },
				value);
		}

		private bool autoClip = true;
		/// <summary>
		/// Gets or sets a value indicating whether descendant graph class 
		/// relies on autotic clipping by Viewport.OutputWithMargin or
		/// does its own clipping.
		/// </summary>
		public bool AutoClip {
			get { return autoClip; }
			set { autoClip = value; }
		}

		private bool autoTranslate = true;
		/// <summary>
		/// Gets or sets a value indicating whether descendant graph class
		/// relies on automatic translation of it, or does its own.
		/// </summary>
		public bool AutoTranslate {
			get { return autoTranslate; }
			set { autoTranslate = value; }
		}

		private RenderTo renderTarget = RenderTo.Screen;
		/// <summary>
		/// Gets or sets a value indicating whether descendant graph class 
		/// uses cached rendering of its content to image, or not.
		/// </summary>
		public RenderTo RenderTarget {
			get { return renderTarget; }
			set { renderTarget = value; }
		}

		private static readonly int cacheSize = 100;
		private sealed class ImageInfo {
			public readonly RenderState State;
			public readonly ImageSource Image;
			public readonly ImageKind Kind;

			public ImageInfo(RenderState state, ImageSource image, ImageKind kind) {
				this.State = state;
				this.Image = image;
				this.Kind = kind;
			}
		}

		Dictionary<Rect, ImageInfo> imageCache = new Dictionary<Rect, ImageInfo>(cacheSize);

		private double xCoeff = 1;
		private double yCoeff = 1;
		private void OnRenderCached(DrawingContext dc) {
			Rect output = viewport.OutputWithMargin;
			Rect visible = viewport.Visible;

			Point startPoint = viewport.Visible.Location;

			double width = visible.Width / xCoeff;
			double height = visible.Height / yCoeff;

			// determine all visible Rects
			double x = GetStart(startPoint.X, width);
			double y = GetStart(startPoint.Y, height);
			int xNum = GetNum(x, visible.X, visible.Width, width);
			int yNum = GetNum(y, visible.Y, visible.Height, height);

			Rect[] bounds = new Rect[xNum * yNum];
			for (int j = 0; j < yNum; j++) {
				for (int i = 0; i < xNum; i++) {
					bounds[j * xNum + i] = new Rect(x + i * width, y + j * height, width, height);
				}
			}

			/*
			if (x != startPoint.X || y != startPoint.Y) {
				bounds = new Rect[4];
				bounds[0] = new Rect(x, y, width, height); // top left
				bounds[1] = new Rect(x + width, y, width, height); // top right
				bounds[2] = new Rect(x, y + height, width, height); // bottom left
				bounds[3] = new Rect(x + width, y + height, width, height); // bottom right
			}
			else {
				bounds = new Rect[1];
				bounds[0] = new Rect(x, y, width, height);
			}
			 */

			for (int i = 0; i < bounds.Length; i++) {
				ImageInfo im = GetCachedImage(bounds[i]);
				DrawImageWithTranslate(dc, im);
				DrawDebugRect(dc, xNum, yNum, bounds, i);
			}
		}

		[Conditional("_DEBUG")]
		private void DrawDebugRect(DrawingContext dc, int xNum, int yNum, Rect[] bounds, int i) {
			Rect bound = bounds[i];
			Rect oBound = bound.Transform(viewport.Visible, viewport.OutputWithMargin);

			Brush brush = Brushes.DarkGreen;
			Pen pen = new Pen(brush, 1);

			dc.DrawRectangle(null, pen, oBound);
			int x = i % xNum;
			int y = i / yNum;
			dc.DrawText(CreateFormattedText("x = " + x), new Point(oBound.X + 5, oBound.Bottom + 5));
			dc.DrawText(CreateFormattedText("y = " + y), new Point(oBound.X + 5, oBound.Bottom + 20));
		}

		private static FormattedText CreateFormattedText(string str) {
			return new FormattedText(str, CultureInfo.InvariantCulture,
				FlowDirection.LeftToRight, new Typeface("Arial"),
				12, Brushes.DarkGreen);
		}

		private int GetNum(double start, double realStart, double length, double step) {
			int num = (int)Math.Ceiling(length / step);
			if (start + num * step < realStart + length) {
				num++;
			}
			return num;
		}

		private void DrawImageWithTranslate(DrawingContext dc, ImageInfo imageInfo) {
			if (imageInfo.Kind == ImageKind.Empty) return;
#if !DEBUG
			if (imageInfo.Kind == ImageKind.BeingRendered) return;
#endif

#if false
			// old & working
			Rect output = imageInfo.State.OutputWithMargin;
			Rect outputBounds = imageInfo.State.RenderVisible.Transform(imageInfo.State.Visible, output);

			double xShift = viewport.Visible.X - imageInfo.State.Visible.X;
			double yShift = viewport.Visible.Y - imageInfo.State.Visible.Y;

			double xScale = output.Width / imageInfo.State.RenderVisible.Width;
			double yScale = output.Height / imageInfo.State.RenderVisible.Height;

			double x = outputBounds.X - xShift * xScale;
			double y = outputBounds.Y + yShift * yScale;

			Rect newBounds = new Rect();
			newBounds.X = Math.Ceiling(x);
			newBounds.Y = Math.Ceiling(y);
			newBounds.Width = Math.Ceiling(outputBounds.Width);
			newBounds.Height = Math.Ceiling(outputBounds.Height);
#else

			Rect output = imageInfo.State.OutputWithMargin;
			Rect outputBounds = imageInfo.State.RenderVisible.Transform(imageInfo.State.Visible, output);

			double shift = imageInfo.State.Visible.Height / imageInfo.State.RenderVisible.Height;
			shift -= 1;
			shift *= imageInfo.State.RenderVisible.Height * imageInfo.State.OutputWithMargin.Height
				/ imageInfo.State.Visible.Height;

			double xShift = viewport.Visible.X - imageInfo.State.Visible.X;
			double yShift = viewport.Visible.Y - imageInfo.State.Visible.Y;

			double xScale = output.Width / imageInfo.State.Visible.Width;
			double yScale = output.Height / imageInfo.State.Visible.Height;

			double x = outputBounds.X - xShift * xScale;
			double y = outputBounds.Y + yShift * yScale - shift;

			Rect newBounds = new Rect();
			newBounds.X = Math.Ceiling(x);
			newBounds.Y = Math.Ceiling(y);
			newBounds.Width = Math.Ceiling(outputBounds.Width);
			newBounds.Height = Math.Ceiling(outputBounds.Height);
#endif

			if (imageInfo.Kind == ImageKind.Real) {
				newBounds.Width = imageInfo.Image.Width;
				newBounds.Height = imageInfo.Image.Height;

				dc.DrawImage(imageInfo.Image, newBounds);
#if _DEBUG
				Brush b = Brushes.Red;
				Pen p = new Pen(b, 0.4);
				p.DashStyle = DashStyles.DashDot;
				dc.DrawRectangle(null, p, newBounds);
#endif
			}
#if _DEBUG
			else if (imageInfo.Kind == ImageKind.BeingRendered) {
				Brush b = new SolidColorBrush(Color.FromArgb(50, 150, 150, 150));
				Pen p = new Pen(b, 1);
				dc.DrawRectangle(b, p, newBounds);
			}
#endif
		}

		private enum ImageKind {
			Real,
			BeingRendered,
			Empty
		}

		private ImageInfo GetCachedImage(Rect bounds) {
			ImageInfo info;
			if (imageCache.ContainsKey(bounds)) {
				return imageCache[bounds];
			}

			RenderState state = CreateRenderState(bounds, RenderTo.Image);

			info = imageCache[bounds] = new ImageInfo(state, null, ImageKind.BeingRendered);

#if !comp
			RenderState state = CreateRenderState(bounds);
			DispatcherOperation operation = Dispatcher.BeginInvoke(
				DispatcherPriority.SystemIdle,
				(ImageCompution)
				delegate() {
					return CreateImageForBounds(state);
				});
#else

			ComputationOperation operation = Computator.BeginInvoke(
				(ComputationCallback)CreateImageForBounds,
				(EventHandler)
				delegate(object sender, EventArgs e) {
					ComputationOperation oper = (ComputationOperation)sender;
					ImageSource image = (ImageSource)oper.Result;
					ImageKind kind = image != null ? ImageKind.Real : ImageKind.Empty;
					imageCache[state.RenderVisible] = new ImageInfo(state, image, kind);

					InvalidateVisual();
				},
				state);
#endif

			return info;
		}

		private ImageSource CreateImageForBounds(IAbortable abortable) {
			RenderState state = (RenderState)abortable;

			Rect output = state.OutputWithMargin;
			Rect renderVisible = state.RenderVisible;

			int bmpWidth = (int)(output.Width / xCoeff);
			int bmpHeight = (int)(output.Height / yCoeff);

			DrawingVisual visual = new DrawingVisual();
			using (DrawingContext dc = visual.RenderOpen()) {
				Point outputStart = renderVisible.Location.Transform(state.Visible, output);
				double x = -outputStart.X;
				double y = -outputStart.Y + output.Bottom - output.Top;

				dc.PushTransform(new TranslateTransform(x, y));
				OnRenderCore(dc, state);
				dc.Pop();
			}
			Rect contentBounds = visual.ContentBounds;
			Rect bmpBounds = new Rect(0, 0, bmpWidth, bmpHeight);

			// rendered visual will not be visible on bitmap
			if (!contentBounds.IntersectsWith(bmpBounds)) {
				// nothing to draw
				return null;
			}

			RenderTargetBitmap bmp = new RenderTargetBitmap(bmpWidth, bmpHeight, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(visual);
			bmp.Freeze();

			return bmp;
		}

		private static double GetStart(double start, double size) {
			double times = Math.Floor(start / size);
			return times * size;
		}

		#endregion

		private RenderState CreateRenderState(Rect renderVisible, RenderTo renderingType) {
			return new RenderState(renderVisible, Viewport.Visible, Viewport.Output, Viewport.OutputWithMargin, renderingType);
		}

		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if (e.Property == VisibilityProperty) {
				MakeDirty();
				InvalidateVisual();
			}
		}

		private ImageSource thumbnail = null;
		public ImageSource Thumbnail {
			get {
				if (!createThumbnail) {
					CreateThumbnail = true;
				}
				return thumbnail;
			}
		}

		private bool createThumbnail = false;
		public bool CreateThumbnail {
			get { return createThumbnail; }
			set {
				if (createThumbnail != value) {
					createThumbnail = value;
					if (value) {
						RenderThumbnail();
					}
					else {
						thumbnail = null;
						RaisePropertyChanged("Thumbnail");
					}
				}
			}
		}

		private bool ShouldCreateThumbnail {
			get { return IsLayer && createThumbnail; }
		}

		private DrawingGroup graphContents = null;
		protected sealed override void OnRender(DrawingContext dc) {
			Rect output = viewport.OutputWithMargin;
			if (output.Width == 0 || output.Height == 0) return;
			if (Visibility != Visibility.Visible) return;

			if (!autoTranslate || IsDirty || renderTarget == RenderTo.Image) {
				shift = new Vector();

				graphContents = new DrawingGroup();
				using (DrawingContext context = graphContents.Append()) {
					if (renderTarget == RenderTo.Screen) {
						RenderState state = CreateRenderState(Viewport.Visible, RenderTo.Screen);
						OnRenderCore(context, state);
					}
					else {
						OnRenderCached(context);
					}
					IsDirty = false;
				}
				if (graphContents.CanFreeze) {
					graphContents.Freeze();
				}
			}

			// thumbnail is not created, if
			// 1) CreateThumbnail is false
			// 2) this graph has IsLayer property, set to false
			if (ShouldCreateThumbnail) {
				RenderThumbnail();
			}

			if (autoClip) {
				dc.PushClip(new RectangleGeometry(output));
			}
			bool translate = autoTranslate && IsTranslated;
			if (translate) {
				dc.PushTransform(new TranslateTransform(shift.X, shift.Y));
			}

			dc.DrawDrawing(graphContents);

			if (translate) {
				dc.Pop();
			}
			if (autoClip) {
				dc.Pop();
			}
		}

		private void RenderThumbnail() {
			if (viewport == null) return;

			Rect output = viewport.OutputWithMargin;
			Rect visible = viewport.Visible;

			DrawingVisual visual = new DrawingVisual();
			using (DrawingContext dc = visual.RenderOpen()) {
				Point outputStart = visible.Location.Transform(visible, output);
				double x = -outputStart.X + shift.X;
				double y = -outputStart.Y + output.Bottom - output.Top + shift.Y;
				bool translate = autoTranslate && IsTranslated;
				if (translate) {
					dc.PushTransform(new TranslateTransform(x, y));
				}

				byte c = 240;
				Brush brush = new SolidColorBrush(Color.FromArgb(120, c, c, c));
				Pen pen = new Pen(Brushes.Black, 1);
				dc.DrawRectangle(brush, pen, output);
				dc.DrawDrawing(graphContents);

				if (translate) {
					dc.Pop();
				}
			}

			RenderTargetBitmap bmp = new RenderTargetBitmap((int)output.Width, (int)output.Height, 96, 96, PixelFormats.Pbgra32);
			bmp.Render(visual);
			thumbnail = bmp;
			RaisePropertyChanged("Thumbnail");
		}

		protected abstract void OnRenderCore(DrawingContext dc, RenderState state);

		#region INotifyPropertyChanged Members

		public event PropertyChangedEventHandler PropertyChanged;
		private void RaisePropertyChanged(string propertyName) {
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion
	}
}
