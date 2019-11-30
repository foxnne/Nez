using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Sprites
{
	/// <summary>
	/// temporary class used when loading a SpriteAtlas and by the sprite atlas editor
	/// </summary>
	internal class AtlasData
	{
		public List<string> Names = new List<string>();
		public List<Rectangle> SourceRects = new List<Rectangle>();
		public List<Vector2> Origins = new List<Vector2>();

		public List<string> AnimationNames = new List<string>();
		public List<List<int>> AnimationFrames = new List<List<int>>();
		public List<int> AnimationFps = new List<int>();

		public Atlas ToAtlas(Texture2D texture)
		{
			var sprites = new AtlasSprite[SourceRects.Count];
			var animations = new AtlasAnimation[AnimationFrames.Count];

			for (var i = 0; i < sprites.Length; i++)
			{
				var sprite = new AtlasSprite(SourceRects[i], Origins[i]);
				sprites[i] = sprite;
			}

			for (var i = 0; i < animations.Length; i++)
			{
				AtlasAnimation frames = new AtlasAnimation(AnimationFrames[i].ToArray(), AnimationFps[i]);
				animations[i] = frames;
			}

			return new Atlas(texture, sprites, animations);
		}

		public void Clear()
		{
			Names.Clear();
			SourceRects.Clear();
			Origins.Clear();
			AnimationNames.Clear();
			AnimationFrames.Clear();
			AnimationFps.Clear();
		}

		public void SaveToFile(string filename)
		{
			if (File.Exists(filename))
				File.Delete(filename);

			using (var writer = new StreamWriter(filename))
			{
				for (var i = 0; i < Names.Count; i++)
				{
					writer.WriteLine(Names[i]);

					var rect = SourceRects[i];
					writer.WriteLine("\t{0},{1},{2},{3}", rect.X, rect.Y, rect.Width, rect.Height);
					writer.WriteLine("\t{0},{1}", Origins[i].X, Origins[i].Y);
				}

				if (AnimationNames.Count > 0)
				{
					writer.WriteLine();

					for (var i = 0; i < AnimationNames.Count; i++)
					{
						writer.WriteLine(AnimationNames[i]);
						writer.WriteLine("\t{0}", AnimationFps[i]);
						writer.WriteLine("\t{0}", string.Join(",", AnimationFrames[i]));
					}
				}
			}
		}
	}

}
