using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
    /// <summary>
    /// Concrete implementation of IRenderableIndex for drawing a single
    /// sprite from an atlas.
    /// </summary>
    public class AtlasSprite : IRenderableIndex
    {
        /// <summary>
        /// Index of the sprite within the atlas.
        /// Calls an event when index changes.
        /// </summary>
        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    if (IndexChanged != null)
                    {
                        IndexChanged(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Handles flipping the sprite.
        /// Calls an event when flip state changes.
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get => _spriteEffects;
            set
            {
                if (_spriteEffects != value)
                {
                    _spriteEffects = value;
                    if (SpriteEffectsChanged != null)
                    {
                        SpriteEffectsChanged(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Color passed to the batcher when drawing this sprite.
        /// </summary>
        public Color Color
        {
            get;
            set;
        } = Color.White;

        private int _index;
        private SpriteEffects _spriteEffects;

        public event EventHandler<EventArgs> IndexChanged;
        public event EventHandler<EventArgs> SpriteEffectsChanged;

        public AtlasSprite (int index)
        {
            _index = index;
        }

        public AtlasSprite () { }

    }
}