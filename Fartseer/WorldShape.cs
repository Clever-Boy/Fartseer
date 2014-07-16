using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer
{
	public abstract class WorldShape : Drawable
	{
		public bool Alive { get; set; }

		protected List<Vertex> verts;
		protected PrimitiveType primitiveType;

		protected double lifetimer;
		protected double lifetime;

		public WorldShape(PrimitiveType primitiveType, double lifetime)
		{
			this.primitiveType = primitiveType;
			this.lifetime = lifetime;
			this.lifetimer = 0;

			Alive = true;
		}

		public virtual void CreateVertices()
		{
			verts = new List<Vertex>();
		}

		public virtual void Update(double frametime)
		{
			if (Alive)
			{
				lifetimer += frametime;
				if (lifetimer >= lifetime)
				{
					lifetimer = 0;
					Alive = false;
					//Console.WriteLine("{0} died", this.GetType().Name);
				}
			}
		}

		public void Draw(RenderTarget target, RenderStates states)
		{
			if (Alive)
			{
				target.Draw(verts.ToArray(), primitiveType, states);
			}
		}
	}

	public class Line : WorldShape
	{
		Vertex start, end;

		public Line(Vector2f start, Vector2f end, Color color, double lifetime)
			: base(PrimitiveType.Lines, lifetime)
		{
			this.start = new Vertex(start, color);
			this.end = new Vertex(end, color);
		}

		public override void CreateVertices()
		{
			base.CreateVertices();

			verts.Add(this.start);
			verts.Add(this.end);
		}

		public override void Update(double frametime)
		{
			base.Update(frametime);

			double ratio = lifetimer / lifetime;
			double amount = 255 - (255 * ratio);
			start.Color.A = (byte)amount;
			end.Color.A = (byte)amount;

			CreateVertices();
		}
	}
}
