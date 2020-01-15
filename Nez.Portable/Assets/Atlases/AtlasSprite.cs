using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.UI;

namespace Nez.Textures
{
    //Value type so collections dont have to have new initializer?
    public struct AtlasSprite
    {
        public int Index { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public Color Color { get; set; }

        public bool FlipHorizontally
        {
            get => (SpriteEffects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
            set => SpriteEffects = value
                ? (SpriteEffects | SpriteEffects.FlipHorizontally)
                : (SpriteEffects & ~SpriteEffects.FlipHorizontally);
        }

        public bool FlipVertically
        {
            get => (SpriteEffects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
            set => SpriteEffects = value
                ? (SpriteEffects | SpriteEffects.FlipVertically)
                : (SpriteEffects & ~SpriteEffects.FlipVertically);
        }

        public AtlasSprite(int index)
        {
            Index = index;
            SpriteEffects = SpriteEffects.None;
            Color = Color.White;

        }
    }
}