using Nez.Tools.Packing.Palettes;

namespace Nez.Tools.Packing.Arguments
{
    public class PaletteArguments
    {
        [Argument(ArgumentType.Required, ShortName = "", HelpText = "Output file name for the image.")]
        public string image;

        [Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Output file name for the map.")]
        public string map;

        [Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Input/output palette width.", DefaultValue = 0)]
        public int w = 0;

        [Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Maximum output combined palette height.", DefaultValue = 0)]
        public int mh = 0;

        [Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Number of output rows to leave blank at the top of the image.", DefaultValue = 1)]
        public int tpad = 1;

        [DefaultArgument(ArgumentType.Multiple, HelpText = "Images to pack.", DefaultValue = new string[] { })]
        public string[] input;


        private PaletteArguments() { }

        public static PaletteArguments Parse(params string[] args)
        {
            var arguments = new PaletteArguments();
            if (Parser.ParseArgumentsWithUsage(args, arguments))
                return arguments;
            return null;
        }

        public PalettePacker.Config ToConfig()
        {
            return new PalettePacker.Config
            {
                PaletteOutputFile = image,
                MapOutputFile = map,
                PaletteWidth = w,
                MaxPaletteHeight = mh,
                TopPadding = tpad,
				InputPaths = input
            };
        }
    }
}
