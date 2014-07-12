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

		public event EventHandler OnRemove;

		public Projectile(Physics physics)
		{
			this.physics = physics;
		}

		public void Remove()
		{
			physics.RemoveBody(body);

			if (OnRemove != null)
				OnRemove(this, EventArgs.Empty);
		}

		public void Init(Vector2 position, float offset, float angle)
		{
			body = physics.CreateBody(BodyType.Dynamic, position, new Vector2(10, 10), "boxAlt");
			//body.IsBullet = true;
			//body.IgnoreGravity = true;
			body.OnCollision += (fix1, fix2, contact) =>
			{
				//this.Remove();
				return true;
			};

			double radian = angle * (Math.PI / 180);
			Vector2 direction = Extensions.RadianToVector((float)radian).ToVector2();
			//Console.WriteLine("{0} {1} {2}", angle, radian, direction);
			body.Position += direction * offset;
			body.ApplyLinearImpulse(direction * 1.5f);
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

			bool failed;
			physics = Parent.GetComponent<Physics>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find Physics in {0}", Parent.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public Projectile CreateProjectile(Vector2 position, float angle)
		{
			Projectile proj;
			// if there are any unused projectiles, use the first unused projectile
			// else create a new one
			if (unusedProjectiles.Count > 0)
			{
				proj = unusedProjectiles[0];
				unusedProjectiles.Remove(proj);
				//Console.WriteLine("Unused projectile found; recreating it (Unused projectiles left: {0})", unusedProjectiles.Count);
			}
			else
			{
				proj = new Projectile(physics);
				proj.OnRemove += (s, e) =>
				{
					projectiles.Remove(proj);
					unusedProjectiles.Add(proj);
				};
				//Console.WriteLine("No unused projectiles found; creating a new one");
			}

			proj.Init(position, 1f, angle);
			projectiles.Add(proj);
			//Console.WriteLine("Projectile created (Projectiles: {0})", projectiles.Count);

			return proj;
		}
	}
}
