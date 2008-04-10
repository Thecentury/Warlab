using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarLab;
using System.Windows.Markup;
using System.Xml;

namespace WorldSerializationSample {
	class Program {
		static void Main(string[] args) {

			NormalDistribution norm = new NormalDistribution();
			double[] nums = new double[10];
			for (int i = 0; i < 10; i++) {
				nums[i] = norm.Generate(0, 1);
			}

			World w = (World)XamlReader.Load(XmlReader.Create(@"..\..\World.xaml"));
			Console.ReadLine();
		}
	}
}
