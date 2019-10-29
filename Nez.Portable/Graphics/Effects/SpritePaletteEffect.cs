using Microsoft.Xna.Framework.Graphics;

namespace Nez
{

    public class SpritePaletteEffect : Effect
    {

        public Texture2D PaletteTexture
        {
            set => _paletteTextureParam.SetValue(value);
        }

        EffectParameter _paletteTextureParam;

        public SpritePaletteEffect() : base (Core.GraphicsDevice, EffectResource.SpritePaletteEffectBytes)
        {
            _paletteTextureParam = Parameters["_paletteTexture"];
        }

        public SpritePaletteEffect (Texture2D paletteTexture) : base (Core.GraphicsDevice, EffectResource.SpritePaletteEffectBytes)
        {
            _paletteTextureParam = Parameters["_paletteTexture"];
            _paletteTextureParam.SetValue(paletteTexture);
        }

    }
    
}