using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer.Components
{
	public class ParticleSpawner : DrawableGameComponent
	{
		public override Vector2f Position
		{
			get
			{
				// WOW WHY DIDN'T I KNOW OF THAT 'as'!
				return (Parent as DrawableGameComponent).Position;
			}
		}

		public bool Active { get; private set; }

		ParticleManager particleManager;

		double particleLifetime;
		Vector2f particleVelocity;
		Vector2f particleSize;
		float particleAngularVelocity;
		string particleTextureName;

		public ParticleSpawner(int initPriority)
			: base(initPriority)
		{
			Visible = false;
		}

		protected override bool Init()
		{
			List<GameComponent> result;
			if (!(result = Game.GetComponents(new ComponentList().Add<ParticleManager>())).Any())
			{
				Console.WriteLine("Cannot find requested components in {0}", Game.GetType().Name);
				return false;
			}

			particleManager = result.Get<ParticleManager>();

			return base.Init();
		}

		public void Activate(Vector2f size, Vector2f velocity, double lifetime, float angularVelocity, string texturename)
		{
			particleSize = size;
			particleLifetime = lifetime;
			particleTextureName = texturename;
			particleVelocity = velocity;
			particleAngularVelocity = angularVelocity;
			Active = true;
		}

		public void Deactivate()
		{
			Active = false;
		}

		public override void Update(double frametime)
		{
			base.Update(frametime);

			if (!Active)
				return;

			SpawnOneParticle(particleSize, particleVelocity, particleTextureName, particleLifetime, particleAngularVelocity);
		}

		public void SpawnOneParticle(Vector2f size, Vector2f velocity, string textureName, double lifetime = 1000, float angularVelocity = 0f)
		{
			//Console.WriteLine("Spawning one particle at {0}", Position);
			particleManager.CreateParticle(Position, size, velocity, textureName, lifetime, angularVelocity);
		}

		public void SpawnMultipleParticles(int count, Vector2f size, Vector2f velocity, string textureName, double lifetime = 1000, float angularVelocity = 0f)
		{
			for (int i = 0; i < count; i++)
				SpawnOneParticle(size, velocity, textureName, lifetime, angularVelocity);
		}
	}
}
