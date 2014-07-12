using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

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

		Sprite sprite;
		Vector2f offset = new Vector2f(16, 0);
		ProjectileManager projectileManager;

		public Weapon(int initPriority)
			: base(initPriority)
		{

		}

		protected override bool Init()
		{
			Texture texture = new Texture(Game.GetComponent<ImageManager>().GetImage("raygun"));
			sprite = new Sprite(texture, new IntRect(0, 0, 70, 70));
			sprite.Origin = new Vector2f(26, 42); // gun handle

			projectileManager = Game.GetComponent<ProjectileManager>();
			if (projectileManager == null)
			{
				Console.WriteLine("Cannot find ProjectileManager in {0}", Game.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public void Fire()
		{
			projectileManager.CreateProjectile((Position + new Vector2f(15, -10)).ToVector2(), sprite.Rotation + 90f);
		}

		public override void Update(double frametime)
		{
			sprite.Position = Position;

			Vector2f mouse = Game.Window.MapPixelToCoords(Mouse.GetPosition(Game.Window));
			float angle = mouse.AngleBetween(Position);
			//Console.WriteLine("{0} {1}", mouse, Position);
			sprite.Rotation = angle;
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
