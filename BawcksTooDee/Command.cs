using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fartseer.Components;

namespace Fartseer
{
	public class Command
	{
		// TODO: maybe add event when command was executed, containing bool on whether the command was excuted or not

		Func<bool> condition;
		Action<Actor> action;

		public Command(Func<bool> condition, Action<Actor> action)
		{
			this.condition = condition;
			this.action = action;
		}

		public bool TryExecute(Actor target)
		{
			if (condition())
			{
				action(target);
				return true;
			}
			return false;
		}
	}
}
