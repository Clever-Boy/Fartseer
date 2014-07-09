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

namespace Fartseer
{
	public static class Extensions
	{
		public static Vector2 ToVector2(this Vector2f vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static Vector2f ToVector2f(this Vector2 vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}

		public static Vector2f ToVector2f(this Vector2i vec)
		{
			return new Vector2f(vec.X, vec.Y);
		}

		public static Vector2 ToVector2(this Vector2i vec)
		{
			return new Vector2(vec.X, vec.Y);
		}

		public static string Repeat(this string str, int times)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < times; i++)
				builder.Append(str);
			return builder.ToString();
		}
	}
}
