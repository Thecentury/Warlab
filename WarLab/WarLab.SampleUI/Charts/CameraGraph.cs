using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScientificStudio.Charting.GraphicalObjects;
using System.Windows.Media;
using System.Windows;
using System.Diagnostics;

namespace WarLab.SampleUI.Charts {
	public class CameraGraph : GraphicalObject {
		public static readonly double cameraAngle = 80; // градусов
		protected override void OnVisibleChanged(Rect newRect, Rect oldRect) {
			height = GetHeigthByBase(newRect.Width);
			//Debug.WriteLine(height);
		}

		public static double GetHeigthByBase(double baseLength) {
			double radians = MathHelper.AngleToRadians(cameraAngle);
			double baseAngle = MathHelper.AngleToRadians((180 - cameraAngle) / 2);

			double side = baseLength / Math.Sin(radians) * Math.Sin(baseAngle);
			double square = 0.5 * baseLength * side * Math.Sin(baseAngle);
			double height = square * 2 / baseLength;
			return height;
		}

		public static readonly double DefaultCameraHeigth = 1000;
		private double height = Distance.FromKilometres(0.5);
		public double CameraHeight {
			get { return height; }
		}

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			// do nothing
		}
	}
}
