using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

using FarseerPhysics.Dynamics;

// again, odd farseer shit
using Microsoft.Xna.Framework;

namespace Fartseer.Components
{
	public class Projectile
	{
		Body body;
		Physics physics;

		public Projectile(Physics physics)
		{
			this.physics = physics;
		}

		public void Remove()
		{
			physics.RemoveBody(body);
		}

		public void Init(Vector2 position, float angle)
		{
			body = physics.CreateBody(BodyType.Dynamic, position, new Vector2(10, 10), "box.png");
			body.IsBullet = true;
		}
	}

	public class ProjectileManager : GameComponent
	{
		List<Projectile> projectiles;
		List<Projectile> unusedProjectiles;

		Physics physics;

		public ProjectileManager(int initPriority)
			: base(initPriority)
		{
		}

		protected override bool Init()
		{
			projectiles = new List<Projectile>();
			unusedProjectiles = new List<Projectile>();
			physics = Parent.GetComponent<Physics>();

			return base.Init();
		}

		public void CreateProjectile(Vector2 position, float angle)
		{
			Projectile proj;
			// if there are any unused projectiles, use the first unused projectile
			// else create a new one
			if (unusedProjectiles.Count > 0)
			{
				proj = unusedProjectiles[0];
				unusedProjectiles.Remove(proj);
			}
			else
				proj = new Projectile(physics);

			proj.Init(position, angle);
			projectiles.Add(proj);
		}
	}
}
