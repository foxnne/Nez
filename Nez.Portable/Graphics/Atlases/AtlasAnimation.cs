using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
    /// <summary>
    /// Concrete implementation of IRenderableIndex for drawing a frame of 
    /// an animation stored in the atlas.
    /// </summary>
    public class AtlasAnimation : IRenderableIndex
    {
        /// <summary>
        /// Index to the drawable frame of the atlas.
        /// Equals the animation indexed using the current frame.
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
        /// Current frame of the animation between 0 and the animation length.
        /// </summary>
        public int CurrentFrame
        {
            get => _currentFrame;
            set
            {
                if (_currentFrame != value)
                {
                    _currentFrame = value;
                    if (FrameChanged != null)
                    {
                        FrameChanged(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Index of the animation stored in the atlas.
        /// Calls an event when index changes.
        /// </summary>
        public int AnimationIndex
        {
            get => _animationIndex;
            set
            {
                if (_animationIndex != value)
                {
                    _animationIndex = value;
                    if (AnimationChanged != null)
                    {
                        AnimationChanged(this, null);
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
        /// Color passed to the batcher when drawing this animation.
        /// </summary>
        public Color Color
        {
            get;
            set;
        } = Color.White;

        private int _index;
        private SpriteEffects _spriteEffects;
        private int _currentFrame;
        private int _animationIndex;


        public AtlasAnimation (int index)
        {
            AnimationIndex = index;
        }

        public AtlasAnimation ()
        {
        }

        /// <summary>
        /// Event fired when the index changes, which is driven by the animation frame.
        /// </summary>
        public event EventHandler<EventArgs> IndexChanged;

        /// <summary>
        /// Event fired when the SpriteEffects flip state changes
        /// </summary>
        public event EventHandler<EventArgs> SpriteEffectsChanged;

        /// <summary>
        /// Event fired when the frame changes.
        /// </summary>
        public event EventHandler<EventArgs> FrameChanged;

        /// <summary>
        /// Event fired when the animation changes.
        /// </summary>
        public event EventHandler<EventArgs> AnimationChanged;
    }
}