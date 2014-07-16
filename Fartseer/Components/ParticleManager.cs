using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fartseer.Components
{
	// this component stores all particles in the game and allows particle spawners to fetch unused particles from here
	public class ParticleManager : GameComponent
	{
		List<Particle> particles;

		public ParticleManager(int initPriority)
			 : base(initPriority)
		{

		}

		public void CreateParticle()
		{

		}
	}
}
