using static Nez.Tools.Packing.Palettes.PalettePacker;
using Nez.Tools.Packing.Arguments;

namespace Nez.Tools.Packing.Palettes.Console
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			var arguments = PaletteArguments.Parse(args);
			if (arguments == null || (arguments != null && arguments.input == null))
				return (int)FailCode.FailedParsingArguments;

			return PackPalettes(arguments.ToConfig());
		}
	}
}
