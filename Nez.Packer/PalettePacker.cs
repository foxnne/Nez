using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Nez.Tools.Packing
{
    public class PalettePacker
    {
        public class Config
        {
            public string PaletteOutputFile;
            public string MapOutputFile;
            public int PaletteWidth;
            public int MaxPaletteHeight;
            public int TopPadding;
            public string[] InputPaths;
        }

        //really ugly quick and dirty config loading :P
        public static Config LoadConfig(Config config)
        {
            foreach (var str in config.InputPaths)
            {
                var files = Directory.GetFiles(str);

                foreach (var file in files)
                {
                    if (file.EndsWith(".config"))
                    {
                        StreamReader reader = new StreamReader(file);
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            //ignore lines with comments
                            if (line.IndexOf("#") > 0) { continue; }

                            var equalsIndex = line.IndexOf("=");
                            if (equalsIndex < 0) { continue; }

                            var field = line.Substring(0, equalsIndex).ToLower();
                            var value = line.Substring(equalsIndex + 1);

                            switch (field)
                            {
                                case "paletteoutputfile":
                                    config.PaletteOutputFile = value;
                                    break;
                                case "mapoutputfile":
                                    config.MapOutputFile = value;
                                    break;
                                case "palettewidth":
                                    config.PaletteWidth = Int32.Parse(value);
                                    break;
                                case "maxpaletteheight":
                                    config.MaxPaletteHeight = Int32.Parse(value);
                                    break;
                                case "toppadding":
                                    config.TopPadding = Int32.Parse(value);
                                    break;
                            }
                        }
                    }
                }
            }
            return config;
        }



        public static int PackPalettes(Config config)
        {
            //original config holds the input paths at minimum
            //search those for an existing .config and replace if exists
            config = LoadConfig(config);

            // compile a list of images
            var images = new List<string>();

            int error = FindImages(config, images);
            if (error != 0) { return error; }

            // make sure we found some images
            if (images.Count == 0)
            {
                System.Console.WriteLine("No images to pack.");
                return (int)FailCode.NoImages;
            }

            // generate our output
            var imagePacker = new ImagePacker();

            // pack the image, generating a map only if desired
            int result = imagePacker.PackPalette(images, config.PaletteWidth, config.MaxPaletteHeight, config.TopPadding, out Bitmap outputImage, out Dictionary<string, int> outputMap);
            if (result != 0)
            {
                System.Console.WriteLine("There was an error making the image sheet.");
                return result;
            }

            // try to save using our exporters
            if (File.Exists(config.PaletteOutputFile))
                File.Delete(config.PaletteOutputFile);

            var imageExtension = Path.GetExtension(config.PaletteOutputFile).Substring(1).ToLower();
            switch (imageExtension)
            {
                case "png":
                    outputImage.Save(config.PaletteOutputFile, ImageFormat.Png);
                    break;
                case "jpg":
                    outputImage.Save(config.PaletteOutputFile, ImageFormat.Jpeg);
                    break;
                case "bmp":
                    outputImage.Save(config.PaletteOutputFile, ImageFormat.Bmp);
                    break;
                default:
                    throw new Exception("Invalid image format for output image");
            }

            if (File.Exists(config.MapOutputFile))
                File.Delete(config.MapOutputFile);

          
            //PaletteMapExporter.Save(config.MapOutputFile, outputMap, config);

            return 0;
        }

        static int FindImages(Config arguments, List<string> images)
        {
            var fileNames = new List<string>();
            foreach (var path in arguments.InputPaths)
            {
                if (Directory.Exists(path))
                {
                    foreach (string file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Where(file => MiscHelper.IsImageFile(file)))
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
            }
            return 0;
        }

        //creates animations based on the file name of the images retrieved in FindImages
        //expects animations to be named similar to: "animation_0" , "animation_1", ...
        //the key element is that all animation frames end in an underscore (_) and then a numeric value

        static int CreateAnimations(Config arguments, List<string> images, Dictionary<string, List<string>> animations)
        {
            foreach (var image in images)
            {
                var imageName = Path.GetFileNameWithoutExtension(image);

                //only consider images with digits at the end of the filename
                if (char.IsDigit(imageName[imageName.Length - 1]))
                {
                    var animationName = imageName.Substring(0, imageName.LastIndexOf('_'));
                    if (animations.TryGetValue(animationName, out var animationFrames))
                    {
                        animationFrames.Add(image);
                    }
                    else
                    {
                        var newAnimation = new List<string>() { image };
                        animations.Add(animationName, newAnimation);
                    }
                }
            }
            return 0;
        }
    }
}
