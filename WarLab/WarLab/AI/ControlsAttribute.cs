using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.AI {
	/// <summary>
	/// Указывает, военные объекты какого типа могут управляться данным ИИ.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
	public sealed class ControlsAttribute : Attribute {
		private readonly Type controllsType;
		public Type ControllsType {
			get { return controllsType; }
		} 

		public ControlsAttribute(Type controllsType) {
			this.controllsType = controllsType;
		}
	}
}
