using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ScientificStudio.Charting.GraphicalObjects.Filters {
	internal sealed class FrequencyFilter : IFilter {
		private Rect output;
		public Rect Output {
			get { return output; }
			set { output = value; }
		}


		#region IFilter Members

		public List<Point> Filter(List<Point> points) {
			List<Point> resultPoints = points;

			if (points.Count > 2 * output.Width) {
				resultPoints = new List<Point>();

				List<Point> currentChain = new List<Point>();
				double currentX = Math.Floor(points[0].X);
				foreach (Point p in points) {
					if (Math.Floor(p.X) == currentX) {
						currentChain.Add(p);
					}
					else {
						// Analyse current chain
						if (currentChain.Count <= 2) {
							resultPoints.AddRange(currentChain);
						}
						else {
							Point first = MinByX(currentChain);
							Point last = MaxByX(currentChain);
							Point min = MinByY(currentChain);
							Point max = MaxByY(currentChain);
							resultPoints.Add(first);

							Point smaller = min.X < max.X ? min : max;
							Point greater = min.X > max.X ? min : max;
							if (smaller != resultPoints.Last()) {
								resultPoints.Add(smaller);
							}
							if (greater != resultPoints.Last()) {
								resultPoints.Add(greater);
							}
							if (last != resultPoints.Last()) {
								resultPoints.Add(last);
							}
						}
						currentChain.Clear();
						currentChain.Add(p);
						currentX = Math.Floor(p.X);
					}
				}
			}

			return resultPoints;
		}

		#endregion

		private static Point MinByX(List<Point> points) {
			Point minPoint = points[0];
			foreach (Point p in points) {
				if (p.X < minPoint.X) {
					minPoint = p;
				}
			}
			return minPoint;
		}

		private static Point MaxByX(List<Point> points) {
			Point maxPoint = points[0];
			foreach (Point p in points) {
				if (p.X > maxPoint.X) {
					maxPoint = p;
				}
			}
			return maxPoint;
		}

		private static Point MinByY(List<Point> points) {
			Point minPoint = points[0];
			foreach (Point p in points) {
				if (p.Y < minPoint.Y) {
					minPoint = p;
				}
			}
			return minPoint;
		}

		private static Point MaxByY(List<Point> points) {
			Point maxPoint = points[0];
			foreach (Point p in points) {
				if (p.Y > maxPoint.Y) {
					maxPoint = p;
				}
			}
			return maxPoint;
		}
	}
}
