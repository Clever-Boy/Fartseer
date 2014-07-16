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
		float angle;

		double lifetimer;
		double lifetime;
		Vector2f velocity;
		float angularVelocity;

		public Particle(double lifetime, Vector2f velocity, float angularVelocity, string textureName, ImageManager imageManager)
		{
			if (imageManager.ImageExists(textureName))
				texture = imageManager.GetTexture(textureName);
			else
				Console.WriteLine("ImageManager doesn't contain image \"{0}\"", textureName);

			Alive = true;
			this.lifetime = lifetime;
			this.velocity = velocity;
			this.angularVelocity = angularVelocity;
			lifetimer = 0;

			size = new Vector2f(texture.Size.X, texture.Size.Y);

			CreateVertices();
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

			Position += velocity;
			angle += angularVelocity;

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

	// this component stores all particles in the game and allows particle spawners to spawn particles
	public class ParticleManager : DrawableGameComponent
	{
		List<Particle> particles;
		ImageManager imageManager;

		public ParticleManager(int initPriority)
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

			particles = new List<Particle>();

			return base.Init();
		}

		Particle GetNewOrUnusedParticle()
		{
			Particle particle = null;
			if ((particle = particles.Find(p => !p.Alive)) != null)
				particles.Remove(particle);

			return particle;
		}

		public void CreateParticle(double lifetime, Vector2f velocity, float angularVelocity, string textureName)
		{
			Particle particle = GetNewOrUnusedParticle();
			particle = new Particle(lifetime, velocity, angularVelocity, textureName, imageManager);
			particles.Add(particle);
		}

		public override void Update(double frametime)
		{
			foreach (Particle particle in particles)
				particle.Update(frametime);

			base.Update(frametime);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			foreach (Particle particle in particles)
				particle.Draw(target, states);

			base.Draw(target, states);
		}
	}
}
