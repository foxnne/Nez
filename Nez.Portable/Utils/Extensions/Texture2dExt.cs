
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Nez
{
    public static class Texture2DExt
    {
        /// <summary>
		/// Returns true if the two textures are of equal width and height.
		/// </summary>
		/// <param name="texture">Texture to compare</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Congruent (this Texture2D t, Texture2D texture)
        {
            return t.Width == texture.Width && t.Height == texture.Height;
        }

    }
    
}