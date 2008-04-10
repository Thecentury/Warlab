using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace WarLab.SampleUI {
	public static class ResourceManager {
		private static readonly Dictionary<string, object> loadedResources = new Dictionary<string, object>();

		public static BitmapImage GetBitmap(string uri) {
			if (String.IsNullOrEmpty(uri))
				throw new ArgumentException("Uri should not be empty or equal to null", "uri");

			if (loadedResources.ContainsKey(uri)) {
				return (BitmapImage)loadedResources[uri];
			}

			BitmapImage source = new BitmapImage(new Uri(uri, UriKind.Relative));
			loadedResources.Add(uri, source);
			return source;
		}
	}
}
