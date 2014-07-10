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
	public class Player : Actor
	{
		public Player(int initPriority)
			: base(initPriority)
		{
		}

		protected override List<GameComponent> GetInitComponents()
		{
			List<GameComponent> components = new List<GameComponent>();
			components.Add(new CenterView());
			return components;
		}

		protected override bool Init()
		{
			Weapon weapon = new Weapon(0);
			EquipWeapon(weapon);

			return base.Init();
		}

		public override List<ICommand> SetupCommands()
		{
			List<ICommand> commands = new List<ICommand>();

			commands.Add(CreateKeyboardCommand(Keyboard.Key.A, (a) => a.Move(MoveDirection.Left, 5)));
			commands.Add(CreateKeyboardCommand(Keyboard.Key.D, (a) => a.Move(MoveDirection.Right, 5)));
			commands.Add(CreateKeyboardCommand(Keyboard.Key.Space, true, (a) => a.Move(MoveDirection.Jump, 7)));
			commands.Add(CreateMouseCommand(Mouse.Button.Left, true, (a, pos) => Parent.GetComponent<Physics>().CreateBody(BodyType.Dynamic, Game.Window.MapPixelToCoords(pos).ToVector2(), new Vector2(32, 32), "boxAlt")));

			return commands;
		}

		public override Body SetupBody(Physics physics)
		{
			Body b = physics.CreateBody(BodyType.Dynamic, new Vector2f(50, 50).ToVector2(), new Vector2(32, 32), "boxAlt");
			b.FixedRotation = true;

			return b;
		}
	}
}
