using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnemyPlanes
{
	public class Args
	{
		EnemyBomber bomber;
		StaticTarget target;
		public Args(EnemyBomber Bomber,StaticTarget Target)
		{
			bomber = Bomber;
			target = Target;
		}
		public EnemyBomber Bomber
		{
			get { return bomber; }
		}
		public StaticTarget Target
		{
			get { return target; }
		}
	}
}
