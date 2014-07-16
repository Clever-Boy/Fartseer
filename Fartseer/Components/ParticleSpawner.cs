using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer.Components
{
	public class Particle : Transformable, Drawable
	{
		public bool Alive { get; set; }

		Texture texture;
		Vector2f size;
		List<Vertex> verts;
		double lifetimer;
		double lifetime;

		public Particle(double lifetime, string textureName, ImageManager imageManager)
		{
			if (imageManager.ImageExists(textureName))
				texture = imageManager.GetTexture(textureName);
			else
				Console.WriteLine("ImageManager doesn't contain image \"{0}\"", textureName);

			Alive = true;
			this.lifetime = lifetime;
			lifetimer = 0;

			size = new Vector2f(texture.Size.X, texture.Size.Y);
		}

		public void CreateVertices()
		{
			verts = new List<Vertex>();

			verts.Add(new Vertex(Position, new Vector2f(0, 0)));
			verts.Add(new Vertex(Position + new Vector2f(size.X, 0), new Vector2f(texture.Size.X, 0)));
			verts.Add(new Vertex(Position + size, new Vector2f(texture.Size.X, texture.Size.Y)));
			verts.Add(new Vertex(Position + new Vector2f(0, size.Y), new Vector2f(0, texture.Size.Y)));
		}

		public void Update(double frametime)
		{
			if (!Alive)
				return;

			lifetimer += frametime;
			if (lifetimer >= lifetime)
			{
				lifetimer = 0;
				Alive = false;
				Console.WriteLine("Particle died");
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (!Alive)
				return;

			states.Texture = texture;
			target.Draw(verts.ToArray(), PrimitiveType.Quads, states);
		}
	}

	public class ParticleSpawner : DrawableGameComponent
	{
		public bool Active { get; private set; }

		ImageManager imageManager;
		ParticleManager particleManager;

		double particleLifetime;
		string particleTextureName;

		public ParticleSpawner(int initPriority)
			: base(initPriority)
		{

		}

		protected override bool Init()
		{
			bool failed = false;
			imageManager = Game.GetComponent<ImageManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ImageManager in {0}", Game.GetType().Name);
				return false;
			}

			particleManager = Game.GetComponent<ParticleManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ParticleManager in {0}", Game.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public void Activate(double lifetime, string texturename)
		{

		}
	}
}
