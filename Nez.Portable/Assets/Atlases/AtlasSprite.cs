using Microsoft.Xna.Framework;

namespace Nez.Sprites
{
    
    public struct AtlasSprite
    {
        public readonly Rectangle Source;
        public Vector2 Origin;

        public AtlasSprite(Rectangle source, Vector2 origin)
        {
            Source = source;
            Origin = origin;
        }

        public AtlasSprite(Rectangle source)
        {
            Source = source;
            Origin = new Vector2(0.5f, 0.5f);
        }
    }


}