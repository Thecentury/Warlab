using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace ScientificStudio.Charting.GraphicalObjects {
	public class Axises : GraphicalObject {
		public Axises() {
			AutoClip = false;
			AutoTranslate = false;
		}

		private Brush gridBrush;
		private Pen gridPen;
		private double gridBrushThickness = 1.5;

		private Brush brush;
		private Pen pen;
		private double axisesShift = 7;
		private double thickness = 2;

		protected override void OnRenderCore(DrawingContext dc, RenderState state) {
			brush = Brushes.Black;
			pen = new Pen(brush, thickness);

			gridBrush = Brushes.LightGray;
			gridPen = new Pen(gridBrush, gridBrushThickness);
            gridPen.DashStyle = DashStyles.Dot;

			double[] horTicks = CalcHorTicks();
			double[] vertTicks = CalcVertTicks();

			DrawGrid(dc, horTicks, vertTicks);

			DrawBorder(dc);

			Rect output = Viewport.OutputWithMargin;
			{
				Rect clip = new Rect(
					output.BottomLeft,
					Viewport.Output.BottomRight
					);
				dc.PushClip(new RectangleGeometry(clip));
				{
					// ticks and values from the left side;
					DrawHorizontalTicks(dc, horTicks);
					DrawHorizontalValues(dc, horTicks);
				}
				dc.Pop();

				clip = new Rect(
				   Viewport.Output.Location,
				   new Size(Viewport.Output.Width - output.Width, output.Height));
				dc.PushClip(new RectangleGeometry(clip));
				{
					// ticks and values from the bottom side;
					DrawVerticalTicks(dc, vertTicks);
					DrawVerticalValues(dc, vertTicks);
				}
				dc.Pop();
			}
		}

		private void DrawBorder(DrawingContext dc) {
			dc.DrawRectangle(null, pen, Viewport.OutputWithMargin);
		}

		private void DrawGrid(DrawingContext dc, double[] horTicks, double[] vertTicks) {
			dc.PushClip(new RectangleGeometry(Viewport.OutputWithMargin));
			Rect output = Viewport.OutputWithMargin;
			for (int i = 0; i < horTicks.Length; i++) {
				double screenX = new Point(horTicks[i], 0).Transform(Viewport.Visible, output).X;
				LineGeometry line = new LineGeometry(
					new Point(screenX, output.Top),
					new Point(screenX, output.Bottom)
				);
				dc.DrawGeometry(gridBrush, gridPen, line);
			}

			for (int i = 0; i < vertTicks.Length; i++) {
				double screenY = new Point(0, vertTicks[i]).Transform(Viewport.Visible, output).Y;
				LineGeometry line = new LineGeometry(
					new Point(output.Left, screenY),
					new Point(output.Right, screenY)
				);
				dc.DrawGeometry(gridBrush, gridPen, line);
			}
			dc.Pop();
		}

		private void DrawVerticalValues(DrawingContext dc, double[] ticks) {
			Point zero = ZeroOnScreen;
			double screenX = Viewport.Output.Left + 2;
			Rect output = Viewport.OutputWithMargin;

			for (int i = 0; i < ticks.Length; i++) {
				FormattedText formattedText = CreateFormattedText(CreateStringForValue(ticks[i], vertRounding));
				double screenY = new Point(0, ticks[i]).Transform(Viewport.Visible, output).Y;
				dc.DrawText(
					formattedText,
					new Point(screenX, screenY - formattedText.Height / 2)
					);
			}
		}

		private void DrawVerticalTicks(DrawingContext dc, double[] ticks) {
			Point zero = ZeroOnScreen;
			Rect output = Viewport.OutputWithMargin;
			double x = output.Left;

			for (int i = 0; i < ticks.Length; i++) {
				double screenY = new Point(0, ticks[i]).Transform(Viewport.Visible, output).Y;
				LineGeometry line = new LineGeometry(
					new Point(x, screenY),
					new Point(x - axisesShift, screenY)
				);
				dc.DrawGeometry(brush, pen, line);
			}
		}

		private static double[] CreateTicks(double start, double realStart, double realFinish, double step) {
			List<double> resForward = new List<double>();
			double x = start;
			while (x <= realFinish) {
				resForward.Add(x);
				x += step;
			}
			List<double> resBackward = new List<double>();
			x = start - step;
			while (x >= realStart) {
				resBackward.Add(x);
				x -= step;
			}
			resBackward.Reverse();
			List<double> res = new List<double>(resForward.Count + resBackward.Count);
			res.AddRange(resBackward);
			res.AddRange(resForward);
			return res.ToArray();
		}

		private FormattedText CreateFormattedText(string text) {
			return new FormattedText(
				text,
				CultureInfo.CurrentCulture,
				FlowDirection.LeftToRight,
				new Typeface("Arial"),
				10,
				brush);
		}

		private static List<int> tickNums = new List<int> { 20, 10, 5, 4, 2 };

		private static int ComputeValuesNum(double start, double finish, double width) {
			double delta = finish - start;

			width *= 1.6;

			int num = (int)(delta / width);

			if (num >= tickNums[0])
				return tickNums[0];
			foreach (int number in tickNums) {
				if (num >= number)
					return number;
			}
			return tickNums[tickNums.Count - 1];
		}

		private static double Round(double num, int round) {
			if (round <= 0) {
				return Math.Round(num, -round);
			}
			else {
				double pow = Math.Pow(10, round - 1);
				double val = pow * Math.Round(num / Math.Pow(10, round - 1));
				return val;
			}
		}

		private static string CreateStringForValue(double value, int round) {
			value = Round(value, round - 2);
			string res = value.ToString();
			return res;
		}

		private int vertRounding;
		private double[] CalcVertTicks() {
			double start = Viewport.Visible.Top;
			double finish = Viewport.Visible.Bottom;

			double delta = finish - start;

			int log1 = (int)Math.Round(Math.Log10(delta));

			double newStart = Round(start, log1);
			double newFinish = Round(finish, log1);
			if (newStart == newFinish) {
				log1--;
				newStart = Round(start, log1);
				newFinish = Round(finish, log1);
			}
			vertRounding = log1;

			FormattedText startText = CreateFormattedText(newStart.ToString());
			FormattedText finishText = CreateFormattedText(newFinish.ToString());

			double height = Math.Max(
				startText.Height,
				finishText.Height);

			Rect output = Viewport.OutputWithMargin;
			int ticksNum = ComputeValuesNum(output.Top, output.Bottom, height);
			double step = (newFinish - newStart) / ticksNum;

			return CreateTicks(newStart, start, finish, step);
		}

		private int horRounding;
		private double[] CalcHorTicks() {
			double start = Viewport.Visible.Left;
			double finish = Viewport.Visible.Right;

			double delta = finish - start;

			int log1 = (int)Math.Round(Math.Log10(delta));

			double newStart = Round(start, log1);
			double newFinish = Round(finish, log1);
			if (newStart == newFinish) {
				log1--;
				newStart = Round(start, log1);
				newFinish = Round(finish, log1);
			}

			horRounding = log1;

			FormattedText startText = CreateFormattedText(newStart.ToString());
			FormattedText finishText = CreateFormattedText(newFinish.ToString());

			double width = Math.Max(
				startText.Width,
				finishText.Width);

			Rect output = Viewport.OutputWithMargin;
			int ticksNum = ComputeValuesNum(output.Left, output.Right, width);
			double step = (newFinish - newStart) / ticksNum;

			return CreateTicks(newStart, start, finish, step);
		}


		private void DrawHorizontalTicks(DrawingContext dc, double[] ticks) {
			Rect output = Viewport.OutputWithMargin;
			double y = output.Bottom;

			for (int i = 0; i < ticks.Length; i++) {
				double screenX = new Point(ticks[i], 0).Transform(Viewport.Visible, output).X;
				LineGeometry line = new LineGeometry(
					new Point(screenX, y),
					new Point(screenX, y + axisesShift)
				);
				dc.DrawGeometry(brush, pen, line);
			}
		}

		private void DrawHorizontalValues(DrawingContext dc, double[] ticks) {
			double screenY = Viewport.Output.Bottom - axisesShift - 2;
			Rect output = Viewport.OutputWithMargin;

			for (int i = 0; i < ticks.Length; i++) {
				FormattedText formattedText = CreateFormattedText(CreateStringForValue(ticks[i], horRounding));
				double screenX = new Point(ticks[i], 0).Transform(Viewport.Visible, output).X;
				dc.DrawText(
					formattedText,
					new Point(screenX - formattedText.Width / 2, screenY - formattedText.Height)
					);
			}
		}
	}
}
