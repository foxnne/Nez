using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
    public class Atlas : IDisposable
    {
        private Texture2D _texture2D;
        public Texture2D Texture2D
        {
            get { return _texture2D; }
            set { _texture2D = _texture2D == null || _texture2D.Congruent(value) ? value : _texture2D; }
        }

        public readonly Rectangle[] SourceRects;
        public readonly Vector2[] Origins;

        public readonly int[][] Animations;
        public readonly int[] Framerates;

        public Atlas(Texture2D texture, Rectangle[] sourceRects, Vector2[] origins, int[][] animations, int[]framerates)
        {
            _texture2D = texture;
            SourceRects = sourceRects;
            Origins = origins;
            Animations = animations;
            Framerates = framerates;
        }

        public void Dispose()
        {
            _texture2D.Dispose();
        }
    }
}
