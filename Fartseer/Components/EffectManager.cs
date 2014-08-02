using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

using FarseerPhysics.Common.PhysicsLogic;
using FarseerPhysics.Dynamics;
using FarseerPhysics;

// farseer what the fuck
using Microsoft.Xna.Framework;

namespace Fartseer.Components
{
	public class EffectManager : GameComponent
	{
		Physics physics;

		RealExplosion explosion;

		public EffectManager(int initPriority)
			: base (initPriority)
		{

		}

		protected override bool Init()
		{
			List<GameComponent> result;
			if (!(result = Game.GetComponents(new ComponentList().Add<Physics>())).Any())
			{
				Console.WriteLine("Cannot find requested components in {0}", Game.GetType().Name);
				return false;
			}

			physics = result.Get<Physics>();

			explosion = new RealExplosion(physics.World);

			return base.Init();
		}

		public void Explode(Vector2f position, float radius, float power) // position and radius are in sim units
		{
			//Console.WriteLine("BOOM!");
			explosion.Activate(ConvertUnits.ToSimUnits(position.ToVector2()), radius, power);
		}
	}
}
