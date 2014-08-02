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
		Vector2f offset;
		List<Vertex> verts;
		Color color;

		double lifetimer;
		double lifetime;
		Vector2f velocity;
		float angularVelocity;

		public Particle(Vector2f position, Vector2f size, ImageManager imageManager, string textureName, Vector2f velocity, double lifetime = 1000, float angularVelocity = 0f)
		{
			if (imageManager.ImageExists(textureName))
				texture = imageManager.GetTexture(textureName);
			else
				Console.WriteLine("ImageManager doesn't contain image \"{0}\"", textureName);

			Alive = true;

			this.Position = position;
			this.size = size;
			this.offset = size / 2;
			this.lifetime = lifetime;
			this.velocity = velocity;
			this.angularVelocity = angularVelocity;

			color = new Color(255, 255, 255, 255);
			lifetimer = 0;

			verts = new List<Vertex>();
			CreateVertices();
		}

		public void CreateVertices()
		{
			verts.Clear();

			verts.Add(new Vertex(new Vector2f(0, 0), color, new Vector2f(0, 0)));
			verts.Add(new Vertex(new Vector2f(size.X, 0), color, new Vector2f(texture.Size.X, 0)));
			verts.Add(new Vertex(size, color, new Vector2f(texture.Size.X, texture.Size.Y)));
			verts.Add(new Vertex(new Vector2f(0, size.Y), color, new Vector2f(0, texture.Size.Y)));
		}

		public void Update(double frametime)
		{
			if (!Alive)
				return;

			Position += velocity;

			Transform.Rotate(angularVelocity);

			double ratio = lifetimer / lifetime;
			color.A = (byte)(255 - (255 * ratio));
			CreateVertices();

			lifetimer += frametime;
			if (lifetimer >= lifetime)
			{
				lifetimer = 0;
				Alive = false;
				//Console.WriteLine("Particle died");
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (!Alive)
				return;

			states.Texture = texture;
			states.Transform *= Transform;
	
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
			List<GameComponent> result;
			if (!(result = Game.GetComponents(new ComponentList().Add<ImageManager>())).Any())
			{
				Console.WriteLine("Cannot find requested components in {0}", Game.GetType().Name);
				return false;
			}

			imageManager = result.Get<ImageManager>();

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

		public void CreateParticle(Vector2f position, Vector2f size, Vector2f velocity, string textureName, double lifetime = 1000, float angularVelocity = 0f)
		{
			Particle particle = GetNewOrUnusedParticle();
			particle = new Particle(position, size, imageManager, textureName, velocity, lifetime, angularVelocity);
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
