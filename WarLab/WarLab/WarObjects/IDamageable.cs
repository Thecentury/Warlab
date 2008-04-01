using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WarLab {
	public interface IDamageable {
		double Health { get; }
		event EventHandler Dead;
	}
}
