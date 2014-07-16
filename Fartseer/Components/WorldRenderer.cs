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
		List<WorldShape> shapes;
		Physics physics;
		
		public WorldRenderer(int initPriority)
			: base(initPriority)
		{
		}

		protected override bool Init()
		{
			ComponentFindResult findResult;
			List<GameComponent> result = Game.GetComponents(new ComponentList().Add<Physics>(), out findResult);
			if (findResult.Failed)
			{
				Console.WriteLine("Cannot find requested components in {0}: {1}", Parent.GetType().Name, String.Join(", ", findResult.FailedComponents.ToArray()));
				return false;
			}

			physics = result.Get<Physics>();

			shapes = new List<WorldShape>();

			return base.Init();
		}

		// used in creating shapes
		// returns the first unused shape and removes it from the shapes list to prepare it for re-addition
		WorldShape GetNewOrUnusedShape()
		{
			WorldShape shape = null;
			if ((shape = shapes.Find(s => !s.Alive)) != null)
				shapes.Remove(shape);

			return shape;
		}

		public void AddLine(Vector2f start, Vector2f end, Color color, double lifetime)
		{
			// GetUnusedShape will either return the first unused shape or null
			// either way, the returned shape is recreated
			WorldShape line = GetNewOrUnusedShape();

			line = new Line(start, end, color, lifetime);
			line.CreateVertices();
			shapes.Add(line);

			//Console.WriteLine("Line added. From: {0}, to: {1}. Shapes: {2}", start, end, shapes.Count);
		}

		public override void Update(double frametime)
		{
			base.Update(frametime);

			foreach (WorldShape shape in shapes)
			{
				shape.Update(frametime);
			}
		}

		public override void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states)
		{
			base.Draw(target, states);

			foreach (WorldShape shape in shapes)
			{
				shape.Draw(target, states);
			}

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
