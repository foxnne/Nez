using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nez.Textures;


namespace Nez.Atlases
{
    public class AtlasRenderer : RenderableComponent
    {
        protected Atlas Atlas { get; private set; }

        [Inspectable]
        private int[] _keys;
        [NotInspectable]
        public int this[int i]
        {
            get => _keys[i];
            set => SetKey(i, value);
        }

        


        private void SetKey (int i, int key)
        {
            _keys[i] = key;
            _areBoundsDirty = true;
        }
    
        public int Length
        {
            get => _keys.Length;
        }

        public override RectangleF Bounds
        {
            get
            {
                if (_areBoundsDirty)
                {
                    _bounds.CalculateBounds(
                        Entity.Position, LocalOffset, Atlas.Origins[_keys[0]],
                        Entity.Scale, Entity.Rotation, Atlas.SourceRects[_keys[0]].Width, Atlas.SourceRects[_keys[0]].Height);
                    _areBoundsDirty = false;
                    
                }
                return _bounds;
            }
        }

        private Color[] _colors; 

        private SpriteEffects[] _spriteEffects;

        public AtlasRenderer (Atlas atlas, params int[] keys)
        {
            Atlas = atlas;
            var effects = new SpriteEffects[keys.Length];
            var colors = new Color[keys.Length];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.White;
            }

            _keys = keys;
            _colors = colors;
            _spriteEffects = effects;
        }

        public override void Render(Batcher batcher, Camera camera)
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                batcher.Draw(Atlas.Texture2D, Entity.Position + LocalOffset, Atlas.SourceRects[_keys[i]],
                _colors[i], Entity.Rotation, Atlas.Origins[_keys[i]], Entity.Scale, _spriteEffects[i], LayerDepth);
                
            }  
        }
    }
}