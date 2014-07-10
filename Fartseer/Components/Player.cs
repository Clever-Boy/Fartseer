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
			return base.Init();
		}

		public override void SetupCommands()
		{
			base.SetupCommands();

			CreateKeyboardCommand(Keyboard.Key.A, (a) => a.Move(MoveDirection.Left, 5));
			CreateKeyboardCommand(Keyboard.Key.D, (a) => a.Move(MoveDirection.Right, 5));
			CreateKeyboardCommand(Keyboard.Key.Space, (a) => a.Move(MoveDirection.Jump, 5));
		}

		public override Body SetupBody(Physics physics)
		{
			Body b = physics.CreateBody(BodyType.Dynamic, new Vector2f(50, 50).ToVector2(), new Vector2(32, 32), "boxAlt");
			b.FixedRotation = true;

			return b;
		}
	}
}
