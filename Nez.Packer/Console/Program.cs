using System;

namespace Nez.Tools.Packing
{
    public static class Program
	{
		public static int Main(string[] args)
		{
			//Read the simple config file to get argument array
			var config = Config.Read(args);
			if (config == null)
			{
				Console.WriteLine("Invalid .config location");
				return (int)FailCode.FailedParsingConfig;
			}
			//Determine packable and pack
			return Packable.Pack(args[0], config);
		}
		
	}
}
