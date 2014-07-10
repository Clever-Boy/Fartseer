using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using Fartseer.Components;

namespace Fartseer
{
	public class KeyboardButtonCommand : ICommand
	{
		public Game Game { get; set; }
		Keyboard.Key key;
		Action<Actor> action;

		public KeyboardButtonCommand(Keyboard.Key key, Action<Actor> action)
		{
			this.key = key;
			this.action = action;
		}

		public bool TryExecute(Actor target)
		{
			if (Keyboard.IsKeyPressed(key))
			{
				action(target);
				return true;
			}
			return false;
		}
	}

	public class MouseButtonCommand : ICommand
	{
		public Game Game { get; set; }
		Mouse.Button button;
		Action<Actor, Vector2i> action;

		public MouseButtonCommand(Mouse.Button button, Action<Actor, Vector2i> action)
		{
			this.button = button;
			this.action = action;
		}

		public bool TryExecute(Actor target)
		{
			if (Mouse.IsButtonPressed(button))
			{
				action(target, Mouse.GetPosition());
				return false;
			}
			return true;
		}
	}
}
