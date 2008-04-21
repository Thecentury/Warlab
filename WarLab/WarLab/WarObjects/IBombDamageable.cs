﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	/// <summary>
	/// Объект может быть поврежден бомбой.
	/// </summary>
	internal interface IBombDamageable : IDamageable {
		void MakeDamage(double damage);
	}
}
