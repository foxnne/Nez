using Microsoft.Xna.Framework;
using Nez.Textures;

namespace Nez.Sprites
{
	public struct AtlasAnimation
    {
        public readonly int[] Frames;
        public readonly int Fps;

        public AtlasAnimation (int[] frames, int fps)
        {
            Frames = frames;
            Fps = fps;
        }
    }
}
