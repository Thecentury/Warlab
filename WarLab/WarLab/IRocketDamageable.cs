using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public interface IRocketDamageable : IDamageable {
		void MakeDamage(double damage);
	}
}
