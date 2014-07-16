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
			bool failed;
			physics = Parent.GetComponent<Physics>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find Physics in {0}", this.GetType().Name);
				return false;
			}

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
