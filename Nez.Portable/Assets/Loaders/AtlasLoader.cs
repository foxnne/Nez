﻿using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
	public static class AtlasLoader
	{
		/// <summary>
		/// parses a .atlas file and loads up a Atlas with it's associated Texture
		/// </summary>
		public static Atlas ParseAtlas(string dataFile, bool premultiplyAlpha = false)
		{
			var atlasData = ParseAtlasData(dataFile, true);
			using (var stream = TitleContainer.OpenStream(dataFile.Replace(".atlas", ".png")))
				return atlasData.ToAtlas(premultiplyAlpha ? TextureUtils.TextureFromStreamPreMultiplied(stream) : Texture2D.FromStream(Core.GraphicsDevice, stream));
		}

		/// <summary>
		/// parses a .atlas file into a temporary AtlasData class. If leaveOriginsRelative is true, origins will be left as 0 - 1 range instead
		/// of multiplying them by the width/height.
		/// </summary>
		internal static AtlasData ParseAtlasData(string dataFile, bool leaveOriginsRelative = false)
		{
			var atlas = new AtlasData();

			var parsingSprites = true;
			var commaSplitter = new char[] { ',' };

			string line = null;
			using (var streamFile = Path.IsPathRooted(dataFile) ? File.OpenRead(dataFile) : TitleContainer.OpenStream(dataFile))
			{
				using (var stream = new StreamReader(streamFile))
				{
					while ((line = stream.ReadLine()) != null)
					{
						// once we hit an empty line we are done parsing sprites so we move on to parsing animations
						if (parsingSprites && string.IsNullOrWhiteSpace(line))
						{
							parsingSprites = false;
							continue;
						}

						if (parsingSprites)
						{
							// source rect
							line = stream.ReadLine();
							var lineParts = line.Split(commaSplitter, StringSplitOptions.RemoveEmptyEntries);
							var rect = new Rectangle(int.Parse(lineParts[0]), int.Parse(lineParts[1]), int.Parse(lineParts[2]), int.Parse(lineParts[3]));
							atlas.SourceRects.Add(rect);

							// origin
							line = stream.ReadLine();
							lineParts = line.Split(commaSplitter, StringSplitOptions.RemoveEmptyEntries);
							var origin = new Vector2(float.Parse(lineParts[0], System.Globalization.CultureInfo.InvariantCulture), float.Parse(lineParts[1], System.Globalization.CultureInfo.InvariantCulture));

							if (leaveOriginsRelative)
								atlas.Origins.Add(origin);
							else
								atlas.Origins.Add(origin * new Vector2(rect.Width, rect.Height));
						}
						else
						{
							// catch the case of a newline at the end of the file
							if (string.IsNullOrWhiteSpace(line))
								break;

							// animation fps
							line = stream.ReadLine();
							var fps = int.Parse(line);
							atlas.Framerates.Add(fps);

							// animation frames
							line = stream.ReadLine();
							var frames = new List<int>();
							atlas.AnimationFrames.Add(frames);
							var lineParts = line.Split(commaSplitter, StringSplitOptions.RemoveEmptyEntries);

							foreach (var part in lineParts)
								frames.Add(int.Parse(part));
						}
					}
				}
			}
			return atlas;
		}
	}
}
