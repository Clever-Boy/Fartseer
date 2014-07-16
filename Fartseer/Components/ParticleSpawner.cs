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
		public bool Active { get; private set; }

		ParticleManager particleManager;

		double particleLifetime;
		Vector2f particleVelocity;
		float particleAngularVelocity;
		string particleTextureName;

		public ParticleSpawner(int initPriority)
			: base(initPriority)
		{
			Visible = false;
		}

		protected override bool Init()
		{
			bool failed = false;
			particleManager = Game.GetComponent<ParticleManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ParticleManager in {0}", Game.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public void Activate(double lifetime, Vector2f velocity, float angularVelocity, string texturename)
		{
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

			SpawnOneParticle(particleLifetime, particleVelocity, particleAngularVelocity, particleTextureName);
		}

		public void SpawnOneParticle(double lifetime, Vector2f velocity, float angularVelocity, string textureName)
		{
			particleManager.CreateParticle(lifetime, velocity, angularVelocity, textureName);
		}

		public void SpawnMultipleParticles(int count, double lifetime, Vector2f velocity, float angularVelocity, string textureName)
		{
			for (int i = 0; i < count; i++)
				SpawnOneParticle(lifetime, velocity, angularVelocity, textureName);
		}
	}
}
