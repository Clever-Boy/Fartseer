using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fartseer.Components
{
	public abstract class Weapon : DrawableGameComponent
	{
		public Weapon(int initPriority)
			: base(initPriority)
		{

		}

		protected override bool Init()
		{
			return base.Init();
		}


	}
}
