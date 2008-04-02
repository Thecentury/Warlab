using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScientificStudio.Charting.GraphicalObjects.Descriptions {
	public sealed class ScientificStudioDescription : Description {
		private string parameter = "";
		public string Parameter {
			get { return parameter; }
			set { parameter = value; }
		}

		private string coord = "";
		public string Coord {
			get { return coord; }
			set { coord = value; }
		}

		private string dateTime = "";
		public string DateTime {
			get { return dateTime; }
			set { dateTime = value; }
		}

		public override string Full {
			get {
				if (dateTime != "") {
					return String.Format("{0}, {1}\n{2}", parameter, coord, dateTime);
				}
				else {
					return String.Format("{0}, {1}", parameter, coord);
				}
			}
		}

		public override string Brief {
			get { return parameter; }
		}
	}
}
