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
		Body body;
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
		public virtual List<ICommand> SetupCommands()
		{
			List<ICommand> commands = new List<ICommand>();
			commands.Add(CreateMouseCommand(Mouse.Button.Left, (a, pos) =>
			{
				if (equippedWeapon != null)
					equippedWeapon.Fire();
			}));
			return commands;
		}

		// these methods provide an easier way to create commands
		protected ICommand CreateKeyboardCommand(Keyboard.Key key, Action<Actor> action)
		{
			return CreateKeyboardCommand(key, false, action);
		}
		protected ICommand CreateKeyboardCommand(Keyboard.Key key, bool once, Action<Actor> action)
		{
			KeyboardButtonCommand cmd = new KeyboardButtonCommand(key, once, action);
			cmd.Game = Game;
			return cmd;
		}
		protected ICommand CreateMouseCommand(Mouse.Button button, Action<Actor, Vector2i> action)
		{
			return CreateMouseCommand(button, false, action);
		}
		protected ICommand CreateMouseCommand(Mouse.Button button, bool once, Action<Actor, Vector2i> action)
		{
			MouseButtonCommand cmd = new MouseButtonCommand(button, once, action);
			cmd.Game = Game;
			return cmd;
		}

		protected override bool Init()
		{
			// maybe not best implementation
			commands = SetupCommands();

			bool failed;
			Physics physics = Parent.GetComponent<Physics>(out failed);
			if (failed)
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
