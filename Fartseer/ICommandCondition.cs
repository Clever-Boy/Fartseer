using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer
{
	public interface ICommandCondition
	{
		bool Check();
	}

	public class KeyboardCondition : ICommandCondition
	{
		Keyboard.Key key;

		public KeyboardCondition(Keyboard.Key key)
		{
			this.key = key;
		}

		public bool Check()
		{
			return Keyboard.IsKeyPressed(key);
		}
	}

	public class MouseCondition : ICommandCondition
	{
		Mouse.Button button;

		public MouseCondition(Mouse.Button button)
		{
			this.button = button;
		}

		public bool Check()
		{
			return Mouse.IsButtonPressed(button);
		}
	}
}
