using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nez.Tools.Packing
{
    public static class ImageHelper
    {
        // the valid extensions for images
        public static readonly string[] AllowedImageExtensions = new[] { "png", "jpg", "bmp", "gif" };

        // determines if a file is an image we accept
        public static bool IsImageFile(string file)
        {
            if (!File.Exists(file))
                return false;

            // ToLower for string comparisons
            string fileLower = file.ToLower();

            // see if the file ends with one of our valid extensions
            foreach (var ext in AllowedImageExtensions)
                if (fileLower.EndsWith(ext))
                    return true;
            return false;
        }

        // stolen from http://en.wikipedia.org/wiki/Power_of_two#Algorithm_to_find_the_next-highest_power_of_two
        public static int FindNextPowerOfTwo(int k)
        {
            k--;
            for (int i = 1; i < sizeof(int) * 8; i <<= 1)
                k = k | k >> i;
            return k + 1;
        }

        /// <summary>
        /// Returns the bounds of the non-zero alpha pixels in a bitmap.
        /// </summary>
        public unsafe static Rectangle Crop(Bitmap bitmap)
        {
            if (Image.GetPixelFormatSize(bitmap.PixelFormat) != 32)
                throw new InvalidOperationException("Crop currently only supports 32 bits per pixel images.");

            var bottom = 0;
            var left = bitmap.Width; // Set the left crop point to the width so that the logic below will set the left value to the first non crop color pixel it comes across.
            var right = 0;
            var top = bitmap.Height; // Set the top crop point to the height so that the logic below will set the top value to the first non crop color pixel it comes across.

            var sourceData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            unsafe
            {
                var sourcePtr = (byte*)sourceData.Scan0;

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var pixel = sourcePtr + (x * 4);

                        // If any of the pixel RGBA values don't match and the crop color is not transparent, or if the crop color is transparent and the pixel A value is not transparent
                        if (pixel[3] != 0)
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
                    sourcePtr += sourceData.Stride;
                }
            }

            bitmap.UnlockBits(sourceData);

            if (left < right && top < bottom)
                return new Rectangle(left, top, right - left, bottom - top);

            return new Rectangle(0, 0, bitmap.Width, bitmap.Height); // Entire image should be cropped, so just return null
        }

        public unsafe static void CopyPixels(Bitmap source, Rectangle sourceRect, Bitmap target, Rectangle targetRect)
        {

            if (Image.GetPixelFormatSize(source.PixelFormat) != 32)
                throw new InvalidOperationException("Copy currently only supports 32 bits per pixel images.");

            if (Image.GetPixelFormatSize(target.PixelFormat) != 32)
                throw new InvalidOperationException("Copy currently only supports 32 bits per pixel images.");

            if (sourceRect.Width != targetRect.Width)
                throw new InvalidOperationException("Target and source rects have difference widths");

            if (sourceRect.Height != targetRect.Height)
                throw new InvalidOperationException("Target and source rects have difference heights");

            var targetBounds = new Rectangle(0, 0, target.Width, target.Height);
            if (!targetBounds.Contains(targetRect))
                throw new InvalidOperationException("Target rect is not within the target image bounds");

            var sourceBounds = new Rectangle(0, 0, source.Width, source.Height);
            if (!sourceBounds.Contains(sourceRect))
                throw new InvalidOperationException("Source rect is not within the source image bounds");

            var sourceData = source.LockBits(sourceRect, ImageLockMode.ReadOnly, source.PixelFormat);
            var targetData = target.LockBits(targetRect, ImageLockMode.WriteOnly, target.PixelFormat);

            var pixelSize = Image.GetPixelFormatSize(source.PixelFormat) / 8;

            unsafe
            {
                // Get the first pixels of the bitmaps
                var sourcePtr = (byte*)sourceData.Scan0;
                var targetPtr = (byte*)targetData.Scan0;

                for (var y = 0; y < sourceRect.Height; y++)
                {
                    // Get the first full lines of the source and target rects
                    byte* sourceLine = sourcePtr + (y * sourceData.Stride);
                    byte* targetLine = targetPtr + (y * targetData.Stride);

                    for (var x = 0; x < (sourceRect.Width * pixelSize); x += pixelSize)
                    {
                        // Copy the pixels over to the target
                        targetLine[x] = sourceLine[x]; //g
                        targetLine[x + 1] = sourceLine[x + 1]; //b
                        targetLine[x + 2] = sourceLine[x + 2]; //r
                        targetLine[x + 3] = sourceLine[x + 3]; //a
                    }
                }
            }
            source.UnlockBits(sourceData);
            target.UnlockBits(targetData);
        }

        public static int FindImages(string rootPath, List<string> images)
        {
            var fileNames = new List<string>();

            if (Directory.Exists(rootPath))
            {
                foreach (string file in Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories).Where(file => IsImageFile(file)))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileNames.Contains(fileName))
                    {
                        System.Console.WriteLine("Two images have the same name: {0} = {1}", file, images[fileNames.IndexOf(fileName)]);
                        images.Clear();
                        return (int)FailCode.ImageNameCollision;
                    }
                    fileNames.Add(fileName);
                    images.Add(file);
                }
            }

            return 0;
        }
    }
}
