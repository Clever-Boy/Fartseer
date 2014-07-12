using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

using FarseerPhysics.Dynamics;
using FarseerPhysics;

// farseer what the fuck
using Microsoft.Xna.Framework;

namespace Fartseer.Components
{
	public class EffectManager : GameComponent
	{
		Physics physics;

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

			return base.Init();
		}

		public void Explode(Vector2f position, float radius, float power, params Body[] ignoreBodies) // position and radius are in sim units
		{
			// source: https://farseerphysics.codeplex.com/discussions/224372
			//Console.WriteLine("BOOM!");
			Vector2 pos = ConvertUnits.ToSimUnits(position.ToVector2());
			foreach (Body body in physics.World.BodyList)
			{
				if (ignoreBodies.Contains(body))
					continue;

				Vector2 dist = body.Position - pos;
				float length = dist.Length();
				if (length <= radius)
				{
					float force = (radius - length) * power;
					Vector2 forceVector = Vector2.Normalize(dist);
					forceVector *= force;

					body.ApplyForce(forceVector);
				}
			}
		}
	}
}
