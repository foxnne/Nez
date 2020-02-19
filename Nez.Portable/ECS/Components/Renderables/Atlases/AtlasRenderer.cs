using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;

namespace Nez.Atlases
{
    public class AtlasRenderer : RenderableComponent
    {
        public Atlas Atlas
        {
            get;
            set;
        }

        public int Index
        {
            get => _renderableIndex.Index;
            set => _renderableIndex.Index = value;
        }

        public override Color Color
        {
            get => _renderableIndex.Color;
            set => _renderableIndex.Color = value;

        }

        public SpriteEffects SpriteEffects
        {
            get => _renderableIndex.SpriteEffects;
            set => _renderableIndex.SpriteEffects = value;
        }

        public override float Width
        {
            get => Atlas.Rectangles[_renderableIndex.Index].Width;
        }

        public override float Height
        {
            get => Atlas.Rectangles[_renderableIndex.Index].Height;
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

                    _bounds.CalculateBounds(Transform.Position, LocalOffset, Atlas.Origins[_renderableIndex.Index],
                                            scale, Transform.Rotation, Width, Height);
                                            
                    _areBoundsDirty = false;
                }
                return _bounds;
            }
        }

        private IRenderableIndex _renderableIndex;

        public AtlasRenderer(Atlas atlas, int index)
        {
            Atlas = atlas;
            _renderableIndex = new AtlasSprite(index);

        }

        public override Component Clone()
        {
            return base.Clone();
        }

        public override void Initialize()
        {
            _renderableIndex.IndexChanged += OnIndexChanged;
        }

        public override void OnRemovedFromEntity()
        {
            _renderableIndex.IndexChanged -= OnIndexChanged;
        }

        public void OnIndexChanged(object sender, EventArgs args)
        {
            _areBoundsDirty = true;
        }

        public void OnSpriteEffectsChanged (object sender, EventArgs args)
        {
            _areBoundsDirty = true;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            batcher.Draw(Atlas, _renderableIndex, Transform.Position + LocalOffset, Transform.Rotation,
                        Transform.Scale, LayerDepth);
        }
    }

}