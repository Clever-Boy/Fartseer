using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartseer.Components;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer
{
	public abstract class CommandAction
	{
		protected Game Game { get; set; }

		public CommandAction(Game game)
		{
			Game = game;
		}

		public virtual void Run(Actor actor)
		{
			throw new NotImplementedException();
		}
	}

	public class KeyboardAction : CommandAction
	{
		Action<Actor> action;

		public KeyboardAction(Action<Actor> action, Game game)
			: base(game)
		{
			this.action = action;
		}

		public override void Run(Actor actor)
		{
			action(actor);
		}
	}

	public class MouseAction : CommandAction
	{
		Action<Actor, Vector2i> action;

		public MouseAction(Action<Actor, Vector2i> action, Game game)
			: base(game)
		{
			this.action = action;
		}

		public override void Run(Actor actor)
		{
			action(actor, Mouse.GetPosition(Game.Window));
		}
	}
}
