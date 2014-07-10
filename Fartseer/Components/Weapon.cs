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
				return ((DrawableGameComponent)Parent).Position;
			}
		}

		Sprite sprite;

		public Weapon(int initPriority)
			: base(initPriority)
		{

		}

		protected override bool Init()
		{
			Texture texture = new Texture(Game.GetComponent<ImageManager>().GetImage("boxAlt"));
			sprite = new Sprite(texture, new IntRect(0, 0, 16, 16));
			sprite.Origin = new Vector2f(8, 8);

			return base.Init();
		}

		public override void Update(double frametime)
		{
			sprite.Position = Position;

			Vector2f mouse = Mouse.GetPosition(Game.Window).ToVector2f();
			float angle = mouse.AngleBetween(Position);
			sprite.Rotation = angle * (180f / (float)Math.PI);
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
