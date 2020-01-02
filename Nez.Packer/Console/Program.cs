namespace Nez.Tools.Packing.Atlases.Console
{
    public static class Program
	{
		public static int Main(string[] args)
		{
			//Read the simple config file to get argument array
			var config = ConfigReader.Read(args);

			//Parse the config just to get the map output type
			var packable = new BasePackable().Parse(config);
			if (packable == null) return (int)FailCode.FailedParsingConfig;

			var atlaspackable = new AtlasPackable().Parse(config);
			if (atlaspackable == null) return (int)FailCode.FailedParsingConfig;

			//Pass in the first argument that is now safely the config path
			return atlaspackable.Pack(args[0]);
		}
	}
}
