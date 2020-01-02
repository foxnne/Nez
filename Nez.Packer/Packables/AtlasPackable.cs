using System;
using Nez.Tools.Packing.Atlases;

namespace Nez.Tools.Packing
{
    public class AtlasPackable : Packable<AtlasPackable>
	{
		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Maximum ouput width.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
		public int maxwidth;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Maximum ouput height.", DefaultValue = Constants.DefaultMaximumSheetWidth)]
		public int maxheight;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Padding between images.", DefaultValue = Constants.DefaultImagePadding)]
		public int padding;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Ensures output dimensions are powers of two.")]
		public bool poweroftwo;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Ensures output is square.")]
		public bool square;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Origin X for the images", DefaultValue = Constants.DefaultOrigin)]
		public float originx;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Origin Y for the images", DefaultValue = Constants.DefaultOrigin)]
		public float originy;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Create animations based on folders. Default: true")]
		public bool createanimations;

		[Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Framerate for any animations", DefaultValue = Constants.DefaultFrameRate)]
		public int framerate;

		[Argument(ArgumentType.Multiple, HelpText = "Images to pack.", DefaultValue = new string[] { })]
		public string[] input;

		[Argument( ArgumentType.AtMostOnce, ShortName = "", HelpText = "Output LOVE2D lua file" )]
		public bool outputlua;

		public override int Pack(string path)
		{
			throw new NotImplementedException();
		}

	}
}
