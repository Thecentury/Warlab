using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab.WarObjects {
	/// <summary>
	/// Интерфейс, который реализуют статические объекты мира, имеющие для врага ценность как цель.
	/// </summary>
	public interface IHasImportance {
		/// <summary>
		/// Возвращает важность объекта. Большему значению соответствует большая важность.
		/// </summary>
		double Importance { get; }
	}
}
