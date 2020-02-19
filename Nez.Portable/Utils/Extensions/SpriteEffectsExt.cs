using Microsoft.Xna.Framework.Graphics;

namespace Nez
{
    /// <summary>
    /// Convenience class to provide methods to the SpriteEffects class for setting 
    /// flags rather than having to use bitshifting.
    /// </summary>
    public static class SpriteEffectsExt
    {
        public static void FlipHorizontally (this SpriteEffects source, bool value)
        {
            source = value ? (source | SpriteEffects.FlipHorizontally) : (source & ~SpriteEffects.FlipHorizontally);
        }

        public static void FlipVertically (this SpriteEffects source, bool value)
        {
            source = value ? (source | SpriteEffects.FlipVertically) : (source & ~SpriteEffects.FlipVertically);
        }

        public static bool IsFlippedHorizontally (this SpriteEffects source)
        {
            return (source & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally;
        }

        public static bool IsFlippedVertically (this SpriteEffects source)
        {
            return (source & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically;
        }

    }
    
}