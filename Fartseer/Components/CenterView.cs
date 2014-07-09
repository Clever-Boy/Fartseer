using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using Fartseer.Components;

namespace Fartseer.Components
{
	public class CenterView : GameComponent
	{
		public CenterView()
			: base(0)
		{
		}

		public override void Update(double frametime)
		{
			Game.SetViewCenter(((DrawableGameComponent)Parent).Position);

			base.Update(frametime);
		}
	}
}
