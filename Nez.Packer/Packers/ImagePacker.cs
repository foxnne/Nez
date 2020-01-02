using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Nez.Tools.Packing
{
    public class ImagePacker
    {
        // various properties of the resulting image
        private bool requirePow2, requireSquare;
        private int padding;
        private int outputWidth, outputHeight;

        // the input list of image files
        private List<string> files;

        // some dictionaries to hold the image sizes and destination rectangles
        private readonly Dictionary<string, Size> imageSizes = new Dictionary<string, Size>();
        private readonly Dictionary<string, Rectangle> imagePlacement = new Dictionary<string, Rectangle>();

        private readonly Dictionary<string, Bitmap> imageBitmaps = new Dictionary<string, Bitmap>();
        

        /// <summary>
        /// Packs a collection of images into a single image.
        /// </summary>
        /// <param name="imageFiles">The list of file paths of the images to be combined.</param>
        /// <param name="requirePowerOfTwo">Whether or not the output image must have a power of two size.</param>
        /// <param name="requireSquareImage">Whether or not the output image must be a square.</param>
        /// <param name="maximumWidth">The maximum width of the output image.</param>
        /// <param name="maximumHeight">The maximum height of the output image.</param>
        /// <param name="imagePadding">The amount of blank space to insert in between individual images.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement rectangles for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public int PackImage(
            IEnumerable<string> imageFiles,
            bool requirePowerOfTwo,
            bool requireSquareImage,
            int maximumWidth,
            int maximumHeight,
            int imagePadding,
            out Bitmap outputImage,
            out Dictionary<string, Rectangle> outputMap)
        {
            files = new List<string>(imageFiles);
            requirePow2 = requirePowerOfTwo;
            requireSquare = requireSquareImage;
            outputWidth = maximumWidth;
            outputHeight = maximumHeight;
            padding = imagePadding;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageBitmaps.Clear();
            imagePlacement.Clear();

            // get the sizes of all the images
            foreach (var image in files)
            {
                var bitmap = Bitmap.FromFile(image) as Bitmap;
                if (bitmap == null)
                    return (int)FailCode.FailedToLoadImage;
                
                bitmap = bitmap.Clone(Crop(bitmap), bitmap.PixelFormat);
                imageBitmaps.Add(image, bitmap);
                //imageSizes.Add(image, bitmap.Size);
            }

            // sort our files by file size so we place large sprites first
            files.Sort(
                (f1, f2) =>
                {
                    Size b1 = imageBitmaps[f1].Size;
                    Size b2 = imageBitmaps[f2].Size;

                    int c = -b1.Width.CompareTo(b2.Width);
                    if (c != 0)
                        return c;

                    c = -b1.Height.CompareTo(b2.Height);
                    if (c != 0)
                        return c;

                    return f1.CompareTo(f2);
                });

            // try to pack the images
            if (!PackImageRectangles())
                return (int)FailCode.FailedToPackImage;

            // make our output image
            outputImage = CreateOutputImage();
            if (outputImage == null)
                return (int)FailCode.FailedToSaveImage;

            // go through our image placements and replace the width/height found in there with
            // each image's actual width/height (since the ones in imagePlacement will have padding)
            var keys = new string[imagePlacement.Keys.Count];
            imagePlacement.Keys.CopyTo(keys, 0);
            foreach (var k in keys)
            {
                // get the actual size
                var s = imageBitmaps[k].Size;

                // get the placement rectangle
                var r = imagePlacement[k];

                // set the proper size
                r.Width = s.Width;
                r.Height = s.Height;

                // insert back into the dictionary
                imagePlacement[k] = r;
            }

            // copy the placement dictionary to the output
            outputMap = new Dictionary<string, Rectangle>();
            foreach (var pair in imagePlacement)
            {
                outputMap.Add(pair.Key, pair.Value);
            }

            // clear our dictionaries just to free up some memory
            //imageSizes.Clear();
            imagePlacement.Clear();
            imageBitmaps.Clear();

            return 0;
        }

        public Rectangle Crop(Bitmap bitmap)
        {
            if (Image.GetPixelFormatSize(bitmap.PixelFormat) != 32)
                throw new InvalidOperationException("Crop currently only supports 32 bits per pixel images.");

            var bottom = 0;
            var left = bitmap.Width; // Set the left crop point to the width so that the logic below will set the left value to the first non crop color pixel it comes across.
            var right = 0;
            var top = bitmap.Height; // Set the top crop point to the height so that the logic below will set the top value to the first non crop color pixel it comes across.

            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            unsafe
            {
                var dataPtr = (byte*)bitmapData.Scan0;

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var rgbPtr = dataPtr + (x * 4);

                        var a = rgbPtr[3];

                        // If any of the pixel RGBA values don't match and the crop color is not transparent, or if the crop color is transparent and the pixel A value is not transparent
                        if (a != 0)
                        {
                            if (x < left)
                                left = x;

                            if (x >= right)
                                right = x + 1;

                            if (y < top)
                                top = y;

                            if (y >= bottom)
                                bottom = y + 1;
                        }
                    }

                    dataPtr += bitmapData.Stride;
                }
            }

            bitmap.UnlockBits(bitmapData);

            if (left < right && top < bottom)
                return new Rectangle(left, top, right - left, bottom - top);

            return new Rectangle(0, 0, bitmap.Width, bitmap.Height); // Entire image should be cropped, so just return null

        }

        /// <summary>
        /// Packs a collection of palette images into a single image.
        /// </summary>
        /// <param name="imageFiles">The list of file paths of the palettes to be combined.</param>
        /// <param name="width">Width of the palettes to pack, as well as the final width.</param>
        /// <param name="height">Height of the final packed image</param>
        /// <param name="topPadding">If true, leaves the top row blank.</param>
        /// <param name="outputImage">The resulting output image.</param>
        /// <param name="outputMap">The resulting output map of placement rectangles for the images.</param>
        /// <returns>0 if the packing was successful, error code otherwise.</returns>
        public int PackPalette(
            IEnumerable<string> imageFiles,
            int width,
            int height,
            int topPadding,
            out Bitmap outputImage,
            out Dictionary<string, int> outputMap)
        {
            files = new List<string>();
            outputWidth = width;
            outputHeight = height;

            outputImage = null;
            outputMap = null;

            // make sure our dictionaries are cleared before starting
            imageSizes.Clear();
            imagePlacement.Clear();

            //only add images that match width and are 1 pixel tall (palette)
            //skips all others, sets to first image width if not passed
            foreach (var image in imageFiles)
            {
                var bitmap = Bitmap.FromFile(image) as Bitmap;
                if (bitmap == null)
                    return (int)FailCode.FailedToLoadImage;
                if (outputWidth <= 0)
                    outputWidth = bitmap.Width;
                if (outputWidth > 0 && bitmap.Width != outputWidth)
                    return (int)FailCode.ImageSizeMismatch;
                if (bitmap.Width == width && bitmap.Height == 1)
                    files.Add(image);
            }

            if (files.Count + topPadding > outputHeight)
                return (int)FailCode.FailedToPackImage;

            outputHeight = files.Count + topPadding;

            // sort our files by file size so we place large sprites first
            files.Sort(
                (f1, f2) =>
                {
                    return f1.CompareTo(f2);
                });

            // add image placements
            int currentRow = topPadding;
            if (currentRow < 0) currentRow = 0;

            outputMap = new Dictionary<string, int>();
            foreach (var image in files)
            {
                imagePlacement.Add(image, new Rectangle(0, currentRow, outputWidth, 1));
                outputMap.Add(image, currentRow);
                currentRow++;
            }

            // make our output image
            outputImage = CreateOutputImage();
            if (outputImage == null)
                return (int)FailCode.FailedToSaveImage;

            // clear our dictionaries just to free up some memory
            imageSizes.Clear();
            imagePlacement.Clear();

            return 0;
        }



        // This method does some trickery type stuff where we perform the TestPackingImages method over and over, 
        // trying to reduce the image size until we have found the smallest possible image we can fit.
        private bool PackImageRectangles()
        {
            // create a dictionary for our test image placements
            Dictionary<string, Rectangle> testImagePlacement = new Dictionary<string, Rectangle>();

            // get the size of our smallest image
            int smallestWidth = int.MaxValue;
            int smallestHeight = int.MaxValue;
            foreach (var size in imageSizes)
            {
                smallestWidth = Math.Min(smallestWidth, size.Value.Width);
                smallestHeight = Math.Min(smallestHeight, size.Value.Height);
            }

            // we need a couple values for testing
            int testWidth = outputWidth;
            int testHeight = outputHeight;

            bool shrinkVertical = false;

            // just keep looping...
            while (true)
            {
                // make sure our test dictionary is empty
                testImagePlacement.Clear();

                // try to pack the images into our current test size
                if (!TestPackingImages(testWidth, testHeight, testImagePlacement))
                {
                    // if that failed...

                    // if we have no images in imagePlacement, i.e. we've never succeeded at PackImages,
                    // show an error and return false since there is no way to fit the images into our
                    // maximum size texture
                    if (imagePlacement.Count == 0)
                        return false;

                    // otherwise return true to use our last good results
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                    testWidth += smallestWidth + padding + padding;
                    testHeight += smallestHeight + padding + padding;
                    continue;
                }

                // clear the imagePlacement dictionary and add our test results in
                imagePlacement.Clear();
                foreach (var pair in testImagePlacement)
                    imagePlacement.Add(pair.Key, pair.Value);

                // figure out the smallest bitmap that will hold all the images
                testWidth = testHeight = 0;
                foreach (var pair in imagePlacement)
                {
                    testWidth = Math.Max(testWidth, pair.Value.Right);
                    testHeight = Math.Max(testHeight, pair.Value.Bottom);
                }

                // subtract the extra padding on the right and bottom
                if (!shrinkVertical)
                    testWidth -= padding;
                testHeight -= padding;

                // if we require a power of two texture, find the next power of two that can fit this image
                if (requirePow2)
                {
                    testWidth = MiscHelper.FindNextPowerOfTwo(testWidth);
                    testHeight = MiscHelper.FindNextPowerOfTwo(testHeight);
                }

                // if we require a square texture, set the width and height to the larger of the two
                if (requireSquare)
                {
                    int max = Math.Max(testWidth, testHeight);
                    testWidth = testHeight = max;
                }

                // if the test results are the same as our last output results, we've reached an optimal size,
                // so we can just be done
                if (testWidth == outputWidth && testHeight == outputHeight)
                {
                    if (shrinkVertical)
                        return true;

                    shrinkVertical = true;
                }

                // save the test results as our last known good results
                outputWidth = testWidth;
                outputHeight = testHeight;

                // subtract the smallest image size out for the next test iteration
                if (!shrinkVertical)
                    testWidth -= smallestWidth;
                testHeight -= smallestHeight;
            }
        }

        private bool TestPackingImages(int testWidth, int testHeight, Dictionary<string, Rectangle> testImagePlacement)
        {
            // create the rectangle packer
            ArevaloRectanglePacker rectanglePacker = new ArevaloRectanglePacker(testWidth, testHeight);

            foreach (var image in files)
            {
                // get the bitmap for this file
                Size size = imageBitmaps[image].Size;

                // pack the image
                Point origin;
                if (!rectanglePacker.TryPack(size.Width + padding, size.Height + padding, out origin))
                {
                    return false;
                }

                // add the destination rectangle to our dictionary
                testImagePlacement.Add(image, new Rectangle(origin.X, origin.Y, size.Width + padding, size.Height + padding));
            }

            return true;
        }

        private Bitmap CreateOutputImage()
        {
            try
            {
                var outputImage = new Bitmap(outputWidth, outputHeight, PixelFormat.Format32bppArgb);

                // draw all the images into the output image
                foreach (var image in files)
                {
                    var location = imagePlacement[image];
                    var bitmap = imageBitmaps[image];
                    if (bitmap == null)
                        return null;

                    // copy pixels over to avoid antialiasing or any other side effects of drawing
                    // the subimages to the output image using Graphics
                    for (int x = 0; x < bitmap.Width; x++)
                        for (int y = 0; y < bitmap.Height; y++)
                            outputImage.SetPixel(location.X + x, location.Y + y, bitmap.GetPixel(x, y));
                }

                return outputImage;
            }
            catch
            {
                return null;
            }
        }
    }
}