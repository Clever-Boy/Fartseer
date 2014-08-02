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

		List<Command> commands;
		protected Body body;
		Weapon equippedWeapon;

		public Actor(int initPriority)
			: base(initPriority)
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

		public void EquipWeapon(Weapon weapon)
		{
			// TODO: better weapon equality checking maybe
			if (equippedWeapon == weapon)
			{
				Console.WriteLine("Cannot equip (TODO: weapon name here); it is already equipped");
				return;
			}
			AddComponent(weapon);
			equippedWeapon = weapon;
		}

		public void UnequipWeapon()
		{
			if (equippedWeapon == null)
			{
				Console.WriteLine("Cannot unequip weapon; there is no weapon equipped");
				return;
			}
			RemoveComponent(equippedWeapon);
			equippedWeapon = null;
		}

		// supposed to be overridden by component
		public virtual List<Command> SetupCommands()
		{
			List<Command> commands = new List<Command>();
			commands.Add(CreateMouseCommand(CommandType.Continuous, Mouse.Button.Left, (a, pos) =>
			{
				if (equippedWeapon != null)
					equippedWeapon.Fire();
			}, 50));
			return commands;
		}

		// these methods provide an easier way to create commands
		protected Command CreateKeyboardCommand(CommandType type, Keyboard.Key key, Action<Actor> action, double timerMax = 1000)
		{
			Command cmd = new Command(type, new KeyboardCondition(key), new KeyboardAction(action, Game), timerMax);
			cmd.Game = Game;
			return cmd;
		}
		protected Command CreateMouseCommand(CommandType type, Mouse.Button button, Action<Actor, Vector2i> action, double timerMax = 1000)
		{
			Command cmd = new Command(type, new MouseCondition(button), new MouseAction(action, Game), timerMax);
			cmd.Game = Game;
			return cmd;
		}

		protected override bool Init()
		{
			// TODO: add fail detection to SetupCommands
			commands = SetupCommands();

			List<GameComponent> result;
			if (!(result = Game.GetComponents(new ComponentList().Add<Physics>())).Any())
			{
				Console.WriteLine("Cannot find requested components in {0}", Game.GetType().Name);
				return false;
			}

			Physics physics = result.Get<Physics>();
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
			foreach (Command cmd in commands)
				cmd.TryExecute(this, Game.Frametime);

			base.Update(frametime);
		}

		public override void Draw(SFML.Graphics.RenderTarget target, SFML.Graphics.RenderStates states)
		{
			base.Draw(target, states);
		}
	}
}
