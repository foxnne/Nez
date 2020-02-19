using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Atlases
{
    public class AtlasStackRenderer : RenderableComponent
    {
        /// <summary>
        /// Indexer for easy layer access
        /// </summary>
        public AtlasSprite this [int i]
        {
            get => Stack[i];
        }

        /// <summary>
        /// The atlas each element of the stack will draw from.
        /// </summary>
        public Atlas Atlas
        {
            get;
            set;
        }

        /// <summary>
        /// Stack of renderable indexes to draw
        /// </summary>
        public AtlasSprite[] Stack
        {
            get;
            private set;
        }

        /// <summary>
        /// Renderable component color, acts as a getter/setter for the full stack.
        /// </summary>
        public override Color Color
        {
            get => Stack[0].Color;
            set
            {
                foreach (var renderableIndex in Stack)
                    renderableIndex.Color = value;
            }
        }

        /// <summary>
        /// Handles flipping of the entire component, acts as a getter/setter for the full stack.
        /// </summary>
        public SpriteEffects SpriteEffects
        {
            get => Stack[0].SpriteEffects;
            set
            {
                foreach (var sprite in Stack)
                    sprite.SpriteEffects = value;
            }
        }

        public override float Width
        {
            get => _stackBounds.Width;
        }

        public override float Height
        {
            get => _stackBounds.Height;
        }

        public override RectangleF Bounds
        {
            get
            {
                if (_areBoundsDirty)
                {
                    var scale = new Vector2
                    (
                        SpriteEffects == SpriteEffects.FlipHorizontally ? Transform.Scale.X * -1 : Transform.Scale.X,
                        SpriteEffects == SpriteEffects.FlipVertically ? Transform.Scale.Y * -1 : Transform.Scale.Y
                    );

                    _bounds.CalculateBounds(Transform.Position, LocalOffset,
                   scale, Transform.Rotation, _stackBounds);

                    _areBoundsDirty = false;
                }

                return _bounds;
            }
        }

        private RectangleF _stackBounds;

        public AtlasStackRenderer(Atlas atlas, params int[] sprites)
        {
            Atlas = atlas;
            Stack = new AtlasSprite[sprites.Length];

            for (int i = 0; i < Stack.Length; i++)
                Stack[i] = new AtlasSprite(sprites[i]);
        }

        public override void Initialize()
        {
            // Calculate the initial AABB of our stack of renderables
            CalculateStackBounds();

            // Subscribe to event to be notified of any changes to our stack
            // to recalculate the stack bounds only when needed
            foreach (var renderableIndex in Stack)
            {
                renderableIndex.IndexChanged += OnIndexChanged;
                renderableIndex.SpriteEffectsChanged += OnSpriteEffectsChanged;
            }

        }

        /// <summary>
        /// Calculates the AABB for the full stack of renderable indexes.
        /// </summary>
        public void CalculateStackBounds()
        {
            var x = float.MaxValue;
            var y = float.MaxValue;
            var width = float.MinValue;
            var height = float.MinValue;

            for (var i = 0; i < Stack.Length; i++)
            {
                x = Math.Min(0 - Atlas.Origins[Stack[i].Index].X, x);
                y = Math.Min(0 - Atlas.Origins[Stack[i].Index].Y, y);
                width = Math.Max(Atlas.Rectangles[Stack[i].Index].Width + x, width + x) - x;
                height = Math.Max(Atlas.Rectangles[Stack[i].Index].Height + y, height + y) - y;
            }

            if (_stackBounds.X != x || _stackBounds.Y != y || _stackBounds.Width != width || _stackBounds.Height != height)
            {
                _stackBounds.X = 0 - x;
                _stackBounds.Y = 0 - y;
                _stackBounds.Width = width;
                _stackBounds.Height = height;

                _areBoundsDirty = true;
            }
        }

        public void OnIndexChanged(object sender, EventArgs args)
        {
            CalculateStackBounds();
        }

        public void OnSpriteEffectsChanged(object sender, EventArgs args)
        {
            CalculateStackBounds();
            _areBoundsDirty = true;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            foreach (var renderableIndex in Stack)
                batcher.Draw(Atlas, renderableIndex, Transform.Position + LocalOffset, Transform.Rotation,
                            Transform.Scale, LayerDepth);
        }


    }

}