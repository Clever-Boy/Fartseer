using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;
using Transform = SFML.Graphics.Transform;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

// this is some odd farseer shit
using Microsoft.Xna.Framework;

namespace Fartseer.Components
{
	public class WorldRenderer : DrawableGameComponent
	{
		Physics physics;
		
		public WorldRenderer(int initPriority)
			: base(initPriority)
		{
		}

		protected override bool Init()
		{
			physics = Parent.GetComponent<Physics>();
			if (physics == null)
			{
				Console.WriteLine("Cannot find Physics in {0}", Parent.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public override void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states)
		{
			base.Draw(target, states);

			foreach (Body body in physics.World.BodyList)
			{
				BodyInfo info = (BodyInfo)body.UserData;

				Vector2 screenPos = ConvertUnits.ToDisplayUnits(body.Position);

				//info.sprite.Position = new Vector2f(screenPos.X, screenPos.Y);
				//info.sprite.Rotation = body.Rotation * (180 / (float)Math.PI);
				//target.Draw(info.sprite);

				Transform transform = Transform.Identity;
				transform.Translate(screenPos.ToVector2f());
				transform.Rotate(body.Rotation * (180 / (float)Math.PI));
				transform.Translate(-info.origin);

				Transform prevTransform = states.Transform;
				states.Transform *= transform;
				states.Texture = info.texture;
				target.Draw(info.verts, PrimitiveType.Quads, states);
				states.Transform = prevTransform;
			}
		}
	}
}
