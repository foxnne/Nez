using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Nez.Tools.Packing.Palettes
{
	public static class PaletteMapExporter
	{
		public static void Save(string filename, Dictionary<string,int> images, PalettePacker.Config arguments )
		{
			
			using (var writer = new StreamWriter(filename))
			{
				foreach (var kvPair in images)
				{
					// write out the name and index for this palette
					writer.WriteLine(Path.GetFileNameWithoutExtension(kvPair.Key));
					writer.WriteLine(kvPair.Value);
				}
			}
		}
	
	}
}