using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Nez.Tools.Packing.Arguments;

namespace Nez.Tools.Packing.Atlases
{
    public class AtlasPacker
    {
        public class Config
        {
            public string AtlasOutputFile;
            public string MapOutputFile;
            public int AtlasMaxWidth = Constants.DefaultMaximumSheetWidth;
            public int AtlasMaxHeight = Constants.DefaultMaximumSheetHeight;
            public int Padding = Constants.DefaultImagePadding;
            public bool IsPowerOfTwo = false;
            public bool IsSquare = false;
            public bool RecurseSubdirectories;
            public float OriginX = Constants.DefaultOrigin;
            public float OriginY = Constants.DefaultOrigin;
            public bool CreateAnimations = true;
            public int FrameRate = Constants.DefaultFrameRate;
            public string[] InputPaths;
            public bool OutputLua;
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
                                case "atlasoutputfile":
                                    config.AtlasOutputFile = value;
                                    break;
                                case "mapoutputfile":
                                    config.MapOutputFile = value;
                                    break;
                                case "atlasmaxwidth":
                                    config.AtlasMaxWidth = Int32.Parse(value);
                                    break;
                                case "atlasmaxheight":
                                    config.AtlasMaxHeight = Int32.Parse(value);
                                    break;
                                case "padding":
                                    config.Padding = Int32.Parse(value);
                                    break;
                                case "ispoweroftwo":
                                    if (value == "true") { config.IsPowerOfTwo = true; }
                                    else config.IsPowerOfTwo = false;
                                    break;
                                case "issquare":
                                    if (value == "true") { config.IsSquare = true; }
                                    else config.IsSquare = false;
                                    break;
                                case "originx":
                                    config.OriginX = float.Parse(value);
                                    break;
                                case "originy":
                                    config.OriginY = float.Parse(value);
                                    break;
                                case "createanimations":
                                    if (value == "true") { config.CreateAnimations = true; }
                                    else config.CreateAnimations = false;
                                    break;
                                case "framerate":
                                    config.FrameRate = Int32.Parse(value);
                                    break;
                                case "outputlua":
                                    if (value == "true") { config.OutputLua = true; }
                                    else config.OutputLua = false;
                                    break;
                            }
                        }
                    }
                }
            }
            return config;
        }



        public static int PackSprites(Config config)
        {
            //original config holds the input paths at minimum
            //search those for an existing .config and replace if exists
            config = LoadConfig(config);

            // compile a list of images
            var animations = new Dictionary<string, List<string>>();
            var images = new List<string>();

            //FindImages(config, images, animations);
            int error = FindImages(config, images);
            if (error != 0) { return error; }


            // make sure we found some images
            if (images.Count == 0)
            {
                System.Console.WriteLine("No images to pack.");
                return (int)FailCode.NoImages;
            }

            if (config.CreateAnimations) error = CreateAnimations(config, images, animations);
            if (error != 0) { return error; }

            // generate our output
            var imagePacker = new ImagePacker();

            // pack the image, generating a map only if desired
            int result = imagePacker.PackImage(images, config.IsPowerOfTwo, config.IsSquare, config.AtlasMaxWidth, config.AtlasMaxHeight, config.Padding, out Bitmap outputImage, out Dictionary<string, Rectangle> outputMap);
            if (result != 0)
            {
                System.Console.WriteLine("There was an error making the image sheet.");
                return result;
            }

            // try to save using our exporters
            if (File.Exists(config.AtlasOutputFile))
                File.Delete(config.AtlasOutputFile);

            var imageExtension = Path.GetExtension(config.AtlasOutputFile).Substring(1).ToLower();
            switch (imageExtension)
            {
                case "png":
                    outputImage.Save(config.AtlasOutputFile, ImageFormat.Png);
                    break;
                case "jpg":
                    outputImage.Save(config.AtlasOutputFile, ImageFormat.Jpeg);
                    break;
                case "bmp":
                    outputImage.Save(config.AtlasOutputFile, ImageFormat.Bmp);
                    break;
                default:
                    throw new Exception("Invalid image format for output image");
            }

            if (config.OutputLua)
                config.MapOutputFile = config.MapOutputFile.Replace(".atlas", ".lua");

            if (File.Exists(config.MapOutputFile))
                File.Delete(config.MapOutputFile);

            if (config.OutputLua)
                LuaMapExporter.Save(config.MapOutputFile, outputMap, animations, outputImage.Width, outputImage.Height, config);
            else
                AtlasMapExporter.Save(config.MapOutputFile, outputMap, animations, config);

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
