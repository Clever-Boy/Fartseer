using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

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
	public class BodyInfo
	{
		//public Sprite sprite;
		public Vertex[] verts;
		public Vector2f origin;
		public Texture texture;

		public bool enabled;

		public BodyInfo()
		{
			enabled = true;
		}
	}

	public class Physics : GameComponent
	{
		public World World { get; private set; }

		ImageManager imageManager;

		public Physics(int initPriority)
			: base(initPriority)
		{
		}

		protected override bool Init()
		{
			World = new World(new Vector2(0, 9.82f));
			ConvertUnits.SetDisplayUnitToSimUnitRatio(32f);

			bool failed;
			imageManager = Parent.GetComponent<ImageManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ImageManager in {0}", Parent.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public override void Update(double frametime)
		{
			World.Step(1 / 30f);

			base.Update(frametime);
		}

		public Body CreateBody(BodyType type, Vector2 position, Vector2 size, string textureName)
		{
			return CreateBody(type, position, size, textureName, false, 1f, 0.7f, 0.2f);
		}
		public Body CreateBody(BodyType type, Vector2 position, Vector2 size, string textureName, bool textureRepeat)
		{
			return CreateBody(type, position, size, textureName, textureRepeat, 1f, 0.7f, 0.2f);
		}
		public Body CreateBody(BodyType type, Vector2 position, Vector2 size, string textureName, bool textureRepeat, float density = 1f, float friction = 0.7f, float restitution = 0.2f)
		{
			Vector2 simSize = ConvertUnits.ToSimUnits(size);
			Vector2 simPos = ConvertUnits.ToSimUnits(position);
			Body body = BodyFactory.CreateRectangle(World, simSize.X, simSize.Y, density, simPos);
			body.BodyType = type;
			body.Friction = friction;
			body.Restitution = restitution;

			if (!imageManager.ImageExists(textureName))
			{
				Console.WriteLine("Cannot create {0} body, texture \"{1}\" does not exist", type.ToString().ToLower(), textureName);
				return null;
			}
			Texture texture = new Texture(imageManager.GetImage(textureName));
			texture.Repeated = textureRepeat;

			BodyInfo info = new BodyInfo();

			////info.sprite = new Sprite(texture, new IntRect(0, 0, (int)size.X, (int)size.Y));
			//info.sprite = new Sprite(texture);
			//info.sprite.Origin = new Vector2f(size.X / 2, size.Y / 2);
			//info.sprite.Scale = new Vector2f(size.X / texture.Size.X, size.Y / texture.Size.Y);

			Vector2f coords = new Vector2f(texture.Size.X, texture.Size.Y);
			if (textureRepeat)
				coords = size.ToVector2f();
			
			Vertex[] verts = new Vertex[]
			{
				new Vertex(new Vector2f(0, 0), Color.White, new Vector2f(0, 0)),
				new Vertex(new Vector2f(size.X, 0), Color.White, new Vector2f(coords.X, 0)),
				new Vertex(new Vector2f(size.X, size.Y), Color.White, new Vector2f(coords.X, coords.Y)),
				new Vertex(new Vector2f(0, size.Y), Color.White, new Vector2f(0, coords.Y))
			};
			info.verts = verts;
			info.origin = new Vector2f(size.X / 2, size.Y / 2);
			info.texture = texture;

			body.UserData = info;

			return body;
		}

		public void RemoveBody(Body body)
		{
			World.RemoveBody(body);
		}
	}
}
