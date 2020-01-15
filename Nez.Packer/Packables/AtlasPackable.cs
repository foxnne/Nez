using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Nez.Tools.Packing
{
    public class AtlasPackable : Packable<AtlasPackable>
    {
        [Argument(ArgumentType.AtMostOnce, ShortName = "mw", HelpText = "Maximum ouput width.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
        public int maxwidth;

        [Argument(ArgumentType.AtMostOnce, ShortName = "mh", HelpText = "Maximum ouput height.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
        public int maxheight;

        [Argument(ArgumentType.AtMostOnce, ShortName = "pad", HelpText = "Padding between images.", DefaultValue = Constants.DefaultImagePadding)]
        public int padding;

        [Argument(ArgumentType.AtMostOnce, ShortName = "pow2", HelpText = "Forces output dimensions to powers of two.")]
        public bool poweroftwo;

        [Argument(ArgumentType.AtMostOnce, ShortName = "sqr", HelpText = "Ensures output is square.")]
        public bool square;

        [Argument(ArgumentType.AtMostOnce, ShortName = "t", HelpText = "Attempts to remove all empty space from input images before packing.", DefaultValue = true)]
        public bool tight;

        [Argument(ArgumentType.AtMostOnce, ShortName = "ox", HelpText = "Origin X for the images", DefaultValue = Constants.DefaultOrigin)]
        public float originx;

        [Argument(ArgumentType.AtMostOnce, ShortName = "oy", HelpText = "Origin Y for the images", DefaultValue = Constants.DefaultOrigin)]
        public float originy;

        [Argument(ArgumentType.AtMostOnce, ShortName = "fps", HelpText = "Framerate for any animations", DefaultValue = Constants.DefaultFrameRate)]
        public int framerate;

        public override string[] mapExtensions => new string[] { ".atlas", ".lua" };

        private List<string> _imagePaths = new List<string>();

        private Dictionary<string, Rectangle> _sourceRectangles = new Dictionary<string, Rectangle>();
        private Dictionary<string, Rectangle> _targetRectangles = new Dictionary<string, Rectangle>();
        private Dictionary<string, Point> _origins = new Dictionary<string, Point>();
        private Dictionary<string, Bitmap> _bitmaps = new Dictionary<string, Bitmap>();
        private Dictionary<string, List<string>> _animations = new Dictionary<string, List<string>>();

        private int _atlasWidth;
        private int _atlasHeight;

        public override int Pack(string path)
        {
            int error = 0;

            // Ensure that all collections are empty
            _sourceRectangles.Clear();
            _targetRectangles.Clear();
            _origins.Clear();
            _bitmaps.Clear();
            _animations.Clear();

            // Get the current map and image extensions
            var mapExtension = Path.GetExtension(outputmap);
            var imageExtension = Path.GetExtension(outputimage);

            // Find our target image paths
            error = ImageHelper.FindImages(Path.GetDirectoryName(path), _imagePaths);
            if (error != 0) { return error; }

            // Make sure we found some images
            if (_imagePaths.Count == 0)
            {
                System.Console.WriteLine("No images to pack.");
                return (int)FailCode.NoImages;
            }

            error = CreateAnimations();
            if (error != 0) { return error; }

            // Try to pack all into an atlas
            error = PackImages(out Bitmap atlas);
            if (error != 0) { return error; }

            if (File.Exists(outputimage))
                File.Delete(outputimage);

            switch (imageExtension)
            {
                case ".png":
                    atlas.Save(outputimage, ImageFormat.Png);
                    break;
                case ".jpg":
                    atlas.Save(outputimage, ImageFormat.Jpeg);
                    break;
                case ".bmp":
                    atlas.Save(outputimage, ImageFormat.Bmp);
                    break;
                default:
                    throw new NotImplementedException(imageExtension + " extension not supported");
            }

            if (File.Exists(outputmap))
                File.Delete(outputmap);

            switch (mapExtension)
            {
                case ".atlas":
                    AtlasMapExporter.Save(outputmap, _targetRectangles, _origins, _animations, framerate);
                    break;
                case ".lua":
                    break;
                default:
                    throw new NotImplementedException(mapExtension + " extension not supported");
            }
            return 0;

        }

        private int CreateAnimations()
        {
            foreach (var path in _imagePaths)
            {
                var imageName = Path.GetFileNameWithoutExtension(path);

                // Only consider images with digits at the end of the filename
                if (char.IsDigit(imageName[imageName.Length - 1]))
                {
                    var animationName = imageName.Substring(0, imageName.LastIndexOf('_'));
                    if (_animations.TryGetValue(animationName, out var animationFrames))
                    {
                        animationFrames.Add(path);
                    }
                    else
                    {
                        var newAnimation = new List<string>() { path };
                        _animations.Add(animationName, newAnimation);
                    }
                }
            }
            return 0;
        }

        private int PackImages(out Bitmap atlas)
        {
            atlas = null;

            //Get our source image rectangles for packing
            foreach (var path in _imagePaths)
            {
                var bitmap = Bitmap.FromFile(path) as Bitmap;
                if (tight)
                    _sourceRectangles.Add(path, ImageHelper.Crop(bitmap));
                else
                    _sourceRectangles.Add(path, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                _bitmaps.Add(path, bitmap);
            }

            // Sort our files by image size so we place large images first
            _imagePaths.Sort(
                (f1, f2) =>
                {
                    Size b1 = _sourceRectangles[f1].Size;
                    Size b2 = _sourceRectangles[f2].Size;

                    int c = -b1.Width.CompareTo(b2.Width);
                    if (c != 0)
                        return c;

                    c = -b1.Height.CompareTo(b2.Height);
                    if (c != 0)
                        return c;

                    return f1.CompareTo(f2);
                });

            // Get a dictionary of target rectangles
            if (!PackRectangles())
                return (int)FailCode.FailedToPackImage;

            //Create output atlas and copy images over to the targets in the atlas
            atlas = new Bitmap(_atlasWidth, _atlasHeight, PixelFormat.Format32bppArgb);

            //Copy the pixels from the source rect and image to the atlas
            //Unsafe, should be fast, potentially could use Parallel.For/ForEach
            //if no rectangles overlap at all

            //Also go ahead and calculate the new origins based on the new rects
            foreach (var path in _imagePaths)
            {
                var targetRect = _targetRectangles[path];
                var sourceRect = _sourceRectangles[path];
                var source = _bitmaps[path];

                targetRect.Width = sourceRect.Width;
                targetRect.Height = sourceRect.Height;

                //Calculate the origin used to draw the target rect at the correct location
                var x = originx > 1 ? originx - sourceRect.X : (originx * source.Width) - sourceRect.X;
                var y = originy > 1 ? originy - sourceRect.Y : (originy * source.Height) - sourceRect.Y;

                _origins.Add(path, new Point((int)Math.Round(x), (int)Math.Round(y)));

                ImageHelper.CopyPixels(source, sourceRect, atlas, targetRect);
            }
            return 0;
        }

        private bool PackRectangles()
        {
            var tempAtlasRectangles = new Dictionary<string, Rectangle>();

            _atlasWidth = maxwidth;
            _atlasHeight = maxheight;

            int smallestWidth = int.MaxValue;
            int smallestHeight = int.MaxValue;

            foreach (var pair in _sourceRectangles)
            {
                smallestWidth = Math.Min(smallestWidth, pair.Value.Width);
                smallestHeight = Math.Min(smallestHeight, pair.Value.Height);
            }

            int testWidth = maxwidth;
            int testHeight = maxheight;

            bool shrinkVertical = false;

            while (true)
            {
                tempAtlasRectangles.Clear();

                if (!TryPackRectangles(testWidth, testHeight, tempAtlasRectangles))
                {
                    // No images were able to be packed, error.
                    if (_targetRectangles.Count == 0)
                    {
                        Console.WriteLine("Unable to fit images within specified size");
                        return false;
                    }

                    // Otherwise return true to use our last good results
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                    testWidth += smallestWidth + padding + padding;
                    testHeight += smallestHeight + padding + padding;
                    continue;
                }

                // Clear the imagePlacement dictionary and add our test results in
                _targetRectangles.Clear();
                foreach (var pair in tempAtlasRectangles)
                    _targetRectangles.Add(pair.Key, pair.Value);

                // Figure out the smallest bitmap that will hold all the images
                testWidth = testHeight = 0;
                foreach (var pair in _targetRectangles)
                {
                    testWidth = Math.Max(testWidth, pair.Value.Right);
                    testHeight = Math.Max(testHeight, pair.Value.Bottom);
                }

                // Subtract the extra padding on the right and bottom
                if (!shrinkVertical)
                    testWidth -= padding;
                testHeight -= padding;

                if (poweroftwo)
                {
                    testWidth = ImageHelper.FindNextPowerOfTwo(testWidth);
                    testHeight = ImageHelper.FindNextPowerOfTwo(testHeight);
                }

                if (square)
                {
                    int max = Math.Max(testWidth, testHeight);
                    testWidth = testHeight = max;
                }

                // If the test results are the same as our last output results, we've reached an optimal size,
                // so we can just be done
                if (testWidth == _atlasWidth && testHeight == _atlasHeight)
                {
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true; //? why is this here?
                }

                // Save the test results as our last known good results
                _atlasWidth = testWidth;
                _atlasHeight = testHeight;

                // Subtract the smallest image size out for the next test iteration
                if (!shrinkVertical)
                    testWidth -= smallestWidth;
                testHeight -= smallestHeight;
            }
        }

        private bool TryPackRectangles(int testWidth, int testHeight, Dictionary<string, Rectangle> tempTargetRects)
        {
            ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);

            foreach (var pair in _sourceRectangles)
            {
                Point placement;
                if (!rectanglePacker.TryPack(pair.Value.Width + padding, pair.Value.Height + padding, out placement))
                {
                    return false;
                }

                // Add the destination rectangle to our dictionary
                tempTargetRects.Add(pair.Key, new Rectangle(placement.X, placement.Y, pair.Value.Width + padding, pair.Value.Height + padding));
            }
            return true;
        }
    }
}
