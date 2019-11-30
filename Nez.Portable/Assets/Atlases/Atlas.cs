using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Sprites
{
	public class Atlas : IDisposable
    {
        private Texture2D _texture;
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = _texture == null || _texture.Congruent(value) ? value : _texture; }
        }

        public AtlasSprite[] Sprites { get; private set;}
        public AtlasAnimation[] Animations { get; private set; }

        public Atlas(Texture2D texture, AtlasSprite[] sprites, AtlasAnimation[] animations)
        {
            _texture = texture;
            Sprites = sprites;
            Animations = animations;
        }

        public Atlas(Texture2D texture, List<AtlasSprite> sprites, List<AtlasAnimation> animations)
        {
            _texture = texture;
            Sprites = sprites.ToArray();
            Animations = animations.ToArray();
        }

        public void Dispose()
        {
            if (Sprites != null)
            {
                _texture.Dispose();
                Sprites = null;
                Animations = null;
            }
        }
    }
}
