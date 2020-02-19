using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
    /// <summary>
    /// Provides an index to the atlas to retrieve rendering data
    /// </summary>
    public interface IRenderableIndex
    {
        /// <summary>
        /// Index for accessing data within the atlas.
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// Handles flipping the renderable index when passed to the batcher.
        /// </summary>
        SpriteEffects SpriteEffects { get; set; }

        /// <summary>
        /// Color passed to the batcher.
        /// </summary>
        Color Color { get; set; }

        event EventHandler<EventArgs> IndexChanged;
        event EventHandler<EventArgs> SpriteEffectsChanged;
    }

}