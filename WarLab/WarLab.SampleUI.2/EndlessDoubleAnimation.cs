using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Animation;
using System.Windows;

namespace WarLab.SampleUI {

	class EndlessDoubleAnimation : DoubleAnimation {
		public EndlessDoubleAnimation() {
			Duration = Duration.Forever;
		}

		protected override Freezable CreateInstanceCore() {
			return new EndlessDoubleAnimation();
		}

		protected override double GetCurrentValueCore(double defaultOriginValue, double defaultDestinationValue, AnimationClock animationClock) {
			return From.Value + By.Value * animationClock.CurrentTime.GetValueOrDefault().TotalSeconds;
		}
	}
}
