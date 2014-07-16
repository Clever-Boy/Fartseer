using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace Fartseer.Components
{
	public class ImageManager : GameComponent
	{
		Dictionary<string, Image> images;
		Dictionary<Image, Texture> textures;

		public ImageManager()
			: this(0)
		{
		}
		public ImageManager(int initPriority)
			: base(initPriority)
		{
			Enabled = false;
		}

		protected override bool Init()
		{
			images = new Dictionary<string, Image>();
			textures = new Dictionary<Image, Texture>();

			string path = "texture";
			string[] files = Directory.GetFiles(path, "*.png");
			Console.WriteLine("Found {0} PNG files in \"{1}\"", files.Length, path);
			foreach (string file in files)
			{
				Image img = new Image(file);
				images.Add(Path.GetFileNameWithoutExtension(file), img);
				Console.WriteLine("{0}\"{1}\" loaded", " ".Repeat(initIndex + 1), file);
			}

			return base.Init();
		}

		public Texture GetTexture(string name)
		{
			if (!ImageExists(name))
				return null;

			if (!textures.ContainsKey(images[name]))
			{
				Image img = images[name];
				Texture texture = new Texture(img);
				textures.Add(img, texture);
				return texture;
			}
			else
				return textures[images[name]];
		}

		public Image GetImage(string name)
		{
			if (images.ContainsKey(name))
				return images[name];

			return null;
		}

		public bool ImageExists(string name)
		{
			return images.ContainsKey(name);
		}
	}
}
