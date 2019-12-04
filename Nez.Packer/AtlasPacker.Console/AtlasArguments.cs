using Nez.Tools.Packing.Atlases;

namespace Nez.Tools.Packing.Arguments
{
    public class AtlasArguments
	{
		[Argument(ArgumentType.Required, ShortName = "", HelpText = "Output file name for the image.")]
		public string image;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Output file name for the map.")]
		public string map;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Maximum ouput width.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
		public int mw = Constants.DefaultMaximumSheetWidth;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Maximum ouput height.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
		public int mh = Constants.DefaultMaximumSheetHeight;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Padding between images.", DefaultValue = Constants.DefaultImagePadding)]
		public int pad = Constants.DefaultImagePadding;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Ensures output dimensions are powers of two.")]
		public bool pow2;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Ensures output is square.")]
		public bool sqr;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Origin X for the images", DefaultValue = Constants.DefaultOrigin)]
		public float originX = Constants.DefaultOrigin;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Origin Y for the images", DefaultValue = Constants.DefaultOrigin)]
		public float originY = Constants.DefaultOrigin;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Create animations based on folders. Default: true")]
		public bool createAnimations = true;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Framerate for any animations", DefaultValue = Constants.DefaultFrameRate)]
		public int fps = Constants.DefaultFrameRate;

		[DefaultArgument(ArgumentType.Multiple, HelpText = "Images to pack.", DefaultValue = new string[] { })]
		public string[] input;

		[Argument( ArgumentType.AtMostOnce, ShortName = "", HelpText = "Output LOVE2D lua file" )]
		public bool lua;

		private AtlasArguments() { }

		public static AtlasArguments Parse(params string[] args)
		{
			var arguments = new AtlasArguments();
			if (Parser.ParseArgumentsWithUsage(args, arguments))
				return arguments;
			return null;
		}

		public AtlasPacker.Config ToConfig()
		{
			return new AtlasPacker.Config
			{
				AtlasOutputFile = image,
				MapOutputFile = map,
				AtlasMaxWidth = mw,
				AtlasMaxHeight = mh,
				Padding = pad,
				IsPowerOfTwo = pow2,
				IsSquare = sqr,
				CreateAnimations = createAnimations,
				OriginX = originX,
				OriginY = originY,
				FrameRate = fps,
				InputPaths = input,
				OutputLua = lua
			};
		}
	}
}
