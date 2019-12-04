using static Nez.Tools.Packing.Atlases.AtlasPacker;
using Nez.Tools.Packing.Arguments;

namespace Nez.Tools.Packing.Atlases.Console
{
	public static class Program
	{
		public static int Main(string[] args)
		{
			var arguments = AtlasArguments.Parse(args);
			if (arguments == null || (arguments != null && arguments.input == null))
				return (int)FailCode.FailedParsingArguments;

			return PackSprites(arguments.ToConfig());
		}
	}
}
