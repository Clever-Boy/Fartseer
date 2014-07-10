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
		bool once = false; // once means the previous execution has to have failed for the next execution to succeed
		bool prev = false;

		public KeyboardButtonCommand(Keyboard.Key key, Action<Actor> action)
		{
			this.key = key;
			this.action = action;
		}
		public KeyboardButtonCommand(Keyboard.Key key, bool once, Action<Actor> action)
			: this(key, action)
		{
			this.once = once;
		}

		public bool TryExecute(Actor target)
		{
			bool result = false;
			if (Keyboard.IsKeyPressed(key))
			{
				//Console.WriteLine("{0} {1}", once, prev);
				if (!once || (once && !prev))
				{
					action(target);
					result = true;
				}
				prev = true;
			}
			else
				prev = false;

			return result;
		}
	}

	public class MouseButtonCommand : ICommand
	{
		public Game Game { get; set; }
		Mouse.Button button;
		Action<Actor, Vector2i> action;
		bool once = false; // see above
		bool prev = false;

		public MouseButtonCommand(Mouse.Button button, Action<Actor, Vector2i> action)
		{
			this.button = button;
			this.action = action;
		}
		public MouseButtonCommand(Mouse.Button button, bool once, Action<Actor, Vector2i> action)
			: this(button, action)
		{
			this.once = once;
		}

		public bool TryExecute(Actor target)
		{
			bool result = false;
			if (Mouse.IsButtonPressed(button))
			{
				//Console.WriteLine("{0} {1}", once, prev);
				if (!once || (once && !prev)) 
				{
					action(target, Mouse.GetPosition(Game.Window));
					result = true;
				}
				prev = true;
			}
			else
				prev = false;

			return result;
		}
	}
}
