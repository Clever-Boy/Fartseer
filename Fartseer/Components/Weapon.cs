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
		public int MaxSpread { get; set; }

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
			List<GameComponent> result;
			if (!(result = Game.GetComponents(new ComponentList().Add<ImageManager>().Add<ProjectileManager>().Add<Physics>().Add<WorldRenderer>())).Any())
			{
				Console.WriteLine("Cannot find requested components in {0}", Game.GetType().Name);
				return false;
			}

			ImageManager imageManager = result.Get<ImageManager>();

			Texture texture = imageManager.GetTexture("raygun");
			sprite = new Sprite(texture, new IntRect(0, 0, 70, 70));
			sprite.Origin = new Vector2f(26, 42); // gun handle

			projectileManager = result.Get<ProjectileManager>();
			physics = result.Get<Physics>();
			worldRenderer = result.Get<WorldRenderer>();

			rand = new Random();

			IsRaycast = true;
			MaxSpread = 8;

			return base.Init();
		}

		public void Fire()
		{
			int spread = rand.Next(-MaxSpread, MaxSpread);
			if (!IsRaycast)
				projectileManager.CreateProjectile(Position.ToVector2(), sprite.Rotation + 90f + spread);
			else
			{
				Vector2f dir = Extensions.RadianToVector((sprite.Rotation + 90f + spread) * ((float)Math.PI / 180));
				Vector2f end = dir * 400f;

				List<RayInfo> hits = physics.Raycast(Position.ToVector2(), (Position + end).ToVector2());
				//Console.WriteLine(hits.Count);

				Vector2f lineEnd = Position + end;
				foreach (RayInfo hit in hits)
				{
					hit.fixture.Body.ApplyLinearImpulse(dir.ToVector2() * 2f);
					lineEnd = FarseerPhysics.ConvertUnits.ToDisplayUnits(hit.point).ToVector2f();
				}

				if (Game.Debug)
					worldRenderer.AddLine(Position, lineEnd, Color.Red, 500);
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
