using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Nez.Textures
{
    /// <summary>
    /// Wrapper for Texture2D that holds all information to draw sprites and animations,
    /// ensuring all sprites referencing the atlas share a single draw call.
    /// Texture swapping is simple, keeping all stored rectangles, UVs and origins intact.
    /// </summary>
    public class Atlas : IDisposable
    {
        
        #region Public Properties

        /// <summary>
        /// The backing texture.
        /// </summary>
        public Texture2D Texture2D
        {
            get { return _texture2D; }
            set { _texture2D = _texture2D == null || _texture2D.Congruent(value) ? value : _texture2D; }
        }

        /// <summary>
        /// Source texture name.
        /// </summary>
        public string Name => _texture2D.Name;

        /// <summary>
        /// Source texture width.
        /// </summary>
        public int Width => _texture2D.Width;

        /// <summary>
        /// Source texture height.
        /// </summary>
        public int Height => _texture2D.Height;

        /// <summary>
        /// Source texture width 0 to 1.
        /// </summary>
        public float InverseWidth => 1.0f / Width;

        /// <summary>
        /// Source texture height 0 to 1.
        /// </summary>
        public float InverseHeight => 1.0f / Height;

        /// <summary>
        /// Individual sprite rectangles.
        /// </summary>
        public Rectangle[] Rectangles { get; private set; }

        /// <summary>
        /// Individual sprite uvs;
        /// </summary>
        public RectangleF[] UVs { get; private set; }

        /// <summary>
        /// Individual sprite origins.
        /// </summary>
        public Vector2[] Origins { get; private set; }

        /// <summary>
        /// Collection of indices of sprites making up an animation.
        /// </summary>
        public int[][] Animations { get; private set; }

        /// <summary>
        /// Individual animation framerates.
        /// </summary>
        public int[] Framerates { get; private set; }

        #endregion

        #region  Private Variables

        private Texture2D _texture2D;

        #endregion

        #region Public Constructors

        public Atlas(Texture2D texture, Rectangle[] rectangles, Vector2[] origins, int[][] animations, int[] framerates)
        {
            Texture2D = texture;
            Rectangles = rectangles ?? new Rectangle[] { texture.Bounds };
            Origins = origins ?? new Vector2[] { new Vector2(texture.Width / 2, texture.Height / 2) };
            Animations = animations;
            Framerates = framerates;
            UVs = new RectangleF[Rectangles.Length];
            UpdateUVs();
        }

        /// <summary>
        /// Creates an atlas from rectangles, also generates origins and uvs.
        /// Origins default to the center.
        /// </summary>
        public Atlas(Texture2D texture, params Rectangle[] rectangles)
        {
            Texture2D = texture;
            Rectangles = rectangles;
            Origins = new Vector2[rectangles.Length];
            for (var i = 0; i < Origins.Length; i++)
            {
                Origins[i].X = rectangles[i].Width / 2;
                Origins[i].Y = rectangles[i].Height / 2;
            }
            UVs = new RectangleF[rectangles.Length];
            UpdateUVs();
        }

        /// <summary>
        /// Creates an atlas from uvs, also generates origins and rectangles.
        /// Origins default to the center.
        /// </summary>
        public Atlas(Texture2D texture, params RectangleF[] uvs)
        {
            Texture2D = texture;
            UVs = uvs;
            Rectangles = new Rectangle[uvs.Length];
            UpdateRectangles();
            Origins = new Vector2[uvs.Length];
            for (var i = 0; i < Origins.Length; i++)
            {
                Origins[i].X = Rectangles[i].Width / 2;
                Origins[i].Y = Rectangles[i].Height / 2;
            }
        }

        /// <summary>
        /// Creates an atlas from a texture alone, treating the texture as a whole as a sprite.
        /// A sprite referencing this atlas will only be able to have an index of 0.
        /// </summary>
        public Atlas(Texture2D texture)
        {
            Texture2D = texture;
            Rectangles = new Rectangle[] { texture.Bounds };
            Origins = new Vector2[] { new Vector2(texture.Width / 2, texture.Height / 2) };
            UVs = new RectangleF[] { new RectangleF() };
            UpdateUVs();
        }

        #endregion

        #region Public Methods

        public void SetRectangle(int index, Rectangle rectangle)
        {
            Rectangles[index] = rectangle;
            UpdateUV(index, InverseWidth, InverseHeight);
        }

        public void SetUV(int index, RectangleF uv)
        {
            UVs[index] = uv;
            UpdateRectangle(index);
        }

        public void SetOrigin(int index, Vector2 origin)
        {
            Origins[index] = origin;
        }

        public void UpdateUVs()
        {
            var inverseTexW = 1.0f / Width;
            var inverseTexH = 1.0f / Height;

            for (var i = 0; i < UVs.Length; i++)
            {
                UpdateUV(i, inverseTexW, inverseTexH);
            }
        }

        public void UpdateUV(int index, float inverseTexW, float inverseTexH)
        {
            var uv = UVs[index];
            var rect = Rectangles[index];

            uv.X = rect.X * inverseTexW;
            uv.Y = rect.Y * inverseTexH;
            uv.Width = rect.Width * inverseTexH;
            uv.Height = rect.Height * inverseTexH;

            UVs[index] = uv;
        }

        private void UpdateRectangles()
        {
            for (var i = 0; i < UVs.Length; i++)
            {
                UpdateRectangle(i);
            }
        }

        private void UpdateRectangle(int index)
        {
            var uv = UVs[index];
            var rect = Rectangles[index];

            rect.X = (int)(uv.X * Width);
            rect.Y = (int)(uv.Y * Height);
            rect.Width = (int)(uv.Width * Width);
            rect.Height = (int)(uv.Height * Height);

            Rectangles[index] = rect;
        }

        /// <summary>
        /// Slices the atlas into rectangles and origins using the current texture.
        /// </summary>
        public void Slice(int cellWidth, int cellHeight, Vector2 origin, bool useAbsoluteOrigin = false)
        {
            var cols = _texture2D.Width / cellWidth;
            var rows = _texture2D.Height / cellHeight;
            var length = cols * rows;

            Rectangles = new Rectangle[length];
            Origins = new Vector2[length];
            UVs = new RectangleF[length];

            for (var x = 0; x < cols; x++)
            {
                for (var y = 0; y < rows; y++)
                {
                    Rectangles[x + y] = new Rectangle(x * cellWidth, y * cellHeight, cellWidth, cellHeight);
                    if (useAbsoluteOrigin)
                        Origins[x + y] = origin;
                    else
                        Origins[x + y] = new Vector2(origin.X * cellWidth, origin.Y * cellHeight);
                }
            }
            UpdateUVs();
        }

        /// <summary>
        /// Slices the atlas into rectangles, uvs and origins using the current texture.
        /// Origins default to center.
        /// </summary>
        public void Slice(int cellWidth, int cellHeight)
        {
            Slice(cellWidth, cellHeight, new Vector2(0.5f, 0.5f));
        }

        /// <summary>
        /// Creates a new atlas from the supplied texture and slices into rectangles, uvs and origins.
        /// Origins default to the center.
        /// </summary>
        public static Atlas Slice (Texture2D texture, int cellWidth, int cellHeight)
        {
            var atlas = new Atlas(texture);
            atlas.Slice(cellWidth, cellHeight);
            return atlas;
        }

        public void Dispose()
        {
            _texture2D.Dispose();
        }

        #endregion

        #region Operators

        public static implicit operator Texture2D(Atlas atlas) => atlas.Texture2D;
        public static explicit operator Atlas(Texture2D texture2D) => new Atlas(texture2D);

        #endregion
    }
}
