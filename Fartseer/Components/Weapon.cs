using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;
using FarseerPhysics.Dynamics;

namespace Fartseer.Components
{
	public class Weapon : DrawableGameComponent
	{
		public override Vector2f Position
		{
			get
			{
				return ((DrawableGameComponent)Parent).Position + offset;
			}
		}
		public bool IsRaycast { get; set; }

		Sprite sprite;
		public Vector2f offset = new Vector2f(16, 0);

		ProjectileManager projectileManager;
		Physics physics;
		WorldRenderer worldRenderer;

		Random rand;

		public Weapon(int initPriority)
			: base(initPriority)
		{
		}

		protected override bool Init()
		{
			bool failed;
			ImageManager imageManager = Game.GetComponent<ImageManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ImageManager in {0}", Game.GetType().Name);
				return false;
			}

			Texture texture = new Texture(imageManager.GetImage("raygun"));
			sprite = new Sprite(texture, new IntRect(0, 0, 70, 70));
			sprite.Origin = new Vector2f(26, 42); // gun handle

			projectileManager = Game.GetComponent<ProjectileManager>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find ProjectileManager in {0}", Game.GetType().Name);
				return false;
			}

			physics = Game.GetComponent<Physics>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find Physics in {0}", Game.GetType().Name);
				return false;
			}

			worldRenderer = Game.GetComponent<WorldRenderer>(out failed);
			if (failed)
			{
				Console.WriteLine("Cannot find WorldRenderer in {0}", Game.GetType().Name);
				return false;
			}

			rand = new Random();
			IsRaycast = true;

			return base.Init();
		}

		public void Fire()
		{
			if (!IsRaycast)
			{
				int spread = rand.Next(-8, 8);
				projectileManager.CreateProjectile(Position.ToVector2(), sprite.Rotation + 90f + spread);
			}
			else
			{
				Vector2f dir = Extensions.RadianToVector((sprite.Rotation + 90f) * ((float)Math.PI / 180));
				Vector2f end = dir * 300f;
				worldRenderer.AddLine(Position, Position + end, Color.Red, 1000);

				List<Fixture> hits = physics.Raycast(Position.ToVector2(), (Position + end).ToVector2());
				//Console.WriteLine(hits.Count);
				foreach (Fixture hit in hits)
				{
					hit.Body.ApplyLinearImpulse(new Microsoft.Xna.Framework.Vector2(0, -10));
				}
			}
		}

		public override void Update(double frametime)
		{
			sprite.Position = Position;

			Vector2f mouse = Game.Window.MapPixelToCoords(Mouse.GetPosition(Game.Window));
			float angle = mouse.AngleBetween(Position);
			//Console.WriteLine("{0} {1}", mouse, Position);
			sprite.Rotation = angle;
			//Console.WriteLine(angle);
			//Console.WriteLine(sprite.Position);

			base.Update(frametime);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			sprite.Draw(target, states);

			base.Draw(target, states);
		}
	}
}
