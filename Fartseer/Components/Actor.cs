using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SFML.Graphics;
using SFML.Window;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

// this is some odd farseer shit
using Microsoft.Xna.Framework;

namespace Fartseer.Components
{
	public enum MoveDirection
	{
		Left,
		Right,
		Jump
	}

	public class Actor : DrawableGameComponent
	{
		public override Vector2f Position
		{
			get
			{
				return ConvertUnits.ToDisplayUnits(body.Position).ToVector2f();
			}

			set
			{
				body.Position = ConvertUnits.ToSimUnits(value.ToVector2());
			}
		}
		public int Health { get; private set; }
		public int MaxHealth { get; private set; }

		List<ICommand> commands;
		List<ICommand> tempCommands;
		Body body;

		public Actor(int initPriority)
			: base(initPriority)
		{
		}
		static Actor()
		{
			
		}

		public virtual void Move(MoveDirection dir, float amount)
		{
			switch (dir)
			{
				case MoveDirection.Left:
					body.LinearVelocity = new Vector2(-amount, body.LinearVelocity.Y);
					break;

				case MoveDirection.Right:
					body.LinearVelocity = new Vector2(amount, body.LinearVelocity.Y);
					break;

				case MoveDirection.Jump:
					body.LinearVelocity = new Vector2(body.LinearVelocity.X, -amount);
					break;
			}
		}

		public void Hurt(int amount)
		{
			Health -= amount;
		}

		// supposed to be overridden by component
		public virtual void SetupCommands()
		{
			tempCommands = new List<ICommand>();
		}
		protected void CreateKeyboardCommand(Keyboard.Key key, Action<Actor> action)
		{
			KeyboardButtonCommand cmd = new KeyboardButtonCommand(key, action);
			cmd.Game = Game;
			tempCommands.Add(cmd);
		}
		protected void CreateMouseCommand(Mouse.Button button, Action<Actor, Vector2i> action)
		{
			MouseButtonCommand cmd = new MouseButtonCommand(button, action);
			cmd.Game = Game;
			tempCommands.Add(cmd);
		}

		protected override bool Init()
		{
			SetupCommands();
			commands = tempCommands;

			Physics physics = Parent.GetComponent<Physics>();
			if (physics == null)
			{
				Console.WriteLine("Cannot find Physics in {0}", Parent.GetType().Name);
				return false;
			}
			body = SetupBody(physics);

			return base.Init();
		}

		// supposed to be overridden by component
		public virtual Body SetupBody(Physics physics)
		{
			throw new NotImplementedException();
		}

		public override void Update(double frametime)
		{
			foreach (ICommand cmd in commands)
				cmd.TryExecute(this);

			base.Update(frametime);
		}

		public override void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states)
		{
			base.Draw(target, states);
		}
	}
}
