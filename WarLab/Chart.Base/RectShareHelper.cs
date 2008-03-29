using System.Windows;

namespace ScientificStudio.Charting {
	internal static class RectShareHelper {
		internal static Rect Combine(Rect source, Rect sharedRect, ViewportRectShare share) {
			Rect res = source;

			if ((share | ViewportRectShare.X) == share) {
				res.X = sharedRect.X;
			}
			if ((share | ViewportRectShare.Y) == share) {
				res.Y = sharedRect.Y;
			}
			if ((share | ViewportRectShare.Width) == share) {
				res.Width = sharedRect.Width;
			}
			if ((share | ViewportRectShare.Height) == share) {
				res.Height = sharedRect.Height;
			}

			return res;
		}
	}
}
