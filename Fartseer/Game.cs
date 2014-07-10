﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using Fartseer.Components;

using FarseerPhysics.Dynamics;

// this is some odd farseer shit
using Microsoft.Xna.Framework;

namespace Fartseer
{
	public class Game : DrawableGameComponent
	{
		public RenderWindow Window { get; private set; }
		public RenderStates RStates { get; private set; }
		//public RenderTexture RTexture { get; private set; }

		public double Frametime { get; private set; }
		public double FPS { get; private set; }

		VideoMode videoMode;
		string title;

		public Game(VideoMode videoMode, string title)
			: base(0)
		{
			this.videoMode = videoMode;
			this.title = title;
			Game = this;
		}

		protected override List<GameComponent> GetInitComponents()
		{
			List<GameComponent> components = new List<GameComponent>();

			components.Add(new Physics(10));
			components.Add(new WorldRenderer(5));
			components.Add(new Player(2));
			components.Add(new ImageManager(20));

			return components;
		}

		protected override bool Init()
		{
			ContextSettings settings = new ContextSettings();
			Window = new RenderWindow(videoMode, title, Styles.Close, settings);
			RStates = new RenderStates(RenderStates.Default);
			//RTexture = new RenderTexture(Window.Size.X, Window.Size.Y);

			Window.SetVerticalSyncEnabled(true);

			Window.Closed += (s, e) =>
			{
				Console.WriteLine("Closing game...");
				Window.Close();
			};

			if (ContainsComponent<Physics>())
			{
				// ground
				GetComponent<Physics>().CreateBody(BodyType.Static, new Vector2(400, 500), new Vector2(800, 70), "grassMid", true);
			}
			else
			{
				Console.WriteLine("Cannot find Physics in {0}", this.GetType().Name);
				return false;
			}

			return base.Init();
		}

		public void Start()
		{
			Console.WriteLine("Starting game...");

			Console.WriteLine("Components:");
			Action<int, List<GameComponent>> ComponentPrint = null;
			ComponentPrint = new Action<int, List<GameComponent>>((index, components) =>
			{
				foreach (GameComponent comp in components)
				{
					Console.WriteLine(" {0}{1}: {2}", " ".Repeat(index), comp.initPriority, comp.ToString());
					if (comp.Components.Count > 0)
						ComponentPrint(index + 1, comp.Components);
				}
			});
			ComponentPrint(0, Components);

			System.Diagnostics.Stopwatch frameTimer = new System.Diagnostics.Stopwatch();
			while (Window.IsOpen())
			{
				frameTimer.Restart();

				Window.DispatchEvents();

				Window.Clear(Color.Transparent);
				//RTexture.Clear(Color.Transparent);

				Update(Frametime);
				Draw(Window, RStates);

				//RTexture.Display();
				//Window.Draw(new Sprite(RTexture.Texture), RStates);
				Window.Display();

				frameTimer.Stop();
				Frametime = frameTimer.Elapsed.TotalMilliseconds;
				FPS = 1 / frameTimer.Elapsed.TotalSeconds;
			}
		}

		public override void Update(double frametime)
		{
			base.Update(frametime);
		}

		public override void Draw(RenderTarget target, RenderStates states)
		{
			base.Draw(target, states);
		}

		public void SetViewCenter(Vector2f pos)
		{
			View view = Window.GetView();
			view.Center = pos;
			Window.SetView(view);
		}
	}
}
