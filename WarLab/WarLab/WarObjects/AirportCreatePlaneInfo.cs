using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	public sealed class AirportCreatePlaneInfo {
		private readonly Type planesType;
		public Type PlanesType {
			get { return planesType; }
		}

		private readonly int planesNum = 0;
		public int PlanesNum {
			get { return planesNum; }
		}

		public AirportCreatePlaneInfo(Type planesType, int planesNum) {
			Verify.IsNonNegative(planesNum);

			if (planesType == null)
				throw new ArgumentNullException("planeType");

			if (!planesType.IsSubclassOf(typeof(Plane)))
				throw new ArgumentException("Указанный тип самолета должен являться наследником типа Plane");

			this.planesType = planesType;
			this.planesNum = planesNum;
		}
	}
}
