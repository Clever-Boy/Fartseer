using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using Fartseer.Components;

namespace Fartseer
{
	public enum CommandType
	{
		Continuous,
		Once,
		Timed
	}

	public class Command
	{
		public Game Game { get; set; }
		public CommandType Type { get; set; }

		public CommandAction action;
		public ICommandCondition condition;

		// for Type.Once
		bool prev = false;

		// for Type.Timed
		double timer = 0;
		protected double max;
		
		public Command(CommandType type, ICommandCondition condition, CommandAction action, double timerMax = 1000)
		{
			this.Type = type;
			this.condition = condition;
			this.action = action;
			this.max = timerMax;
		}
		
		public bool TryExecute(Actor actor, double frametime)
		{
			bool cond = condition.Check();
			bool result = false;
			if (cond)
			{
				switch (Type)
				{
					case CommandType.Once:
						//Console.WriteLine(prev);
						if (!prev)
						{
							action.Run(actor);
							result = true;
						}
						prev = true;
						break;

					case CommandType.Continuous:
						action.Run(actor);
						result = true;
						break;

					case CommandType.Timed:
						timer += frametime;
						if (timer >= max)
						{
							timer = 0;
							action.Run(actor);
							result = true;
						}
						break;
				}
			}
			else
				prev = false;

			return result;
		}
	}
}
