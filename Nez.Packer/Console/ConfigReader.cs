
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Nez.Tools.Packing
{
    public class ConfigReader
    {
        public static string[] Read(string[] arguments)
        {
            var config = "";
            if (arguments != null && arguments[0] != null)
                config = arguments[0];

            if (!File.Exists(config) || Path.GetExtension(config) != ".config")
                return null;

            var args = new List<string>();

            StreamReader reader = new StreamReader(config);
            string argument;

            while ((argument = reader.ReadLine()) != null)
            {
                //Ignore blank lines and lines without delimiters
                if (string.IsNullOrWhiteSpace(argument)) continue;
                if (argument.IndexOfAny(new char[] { ':', '=' }, 1) == -1) continue;

                //Remove any whitespace and make all lowercase
                argument = argument.ToLowerInvariant();
                argument = argument.Replace(" ", string.Empty);

                //Ignore comments
                if (!char.IsLetter(argument[0])) continue;

                args.Add(argument);
            }
            return args.ToArray();
        }
    }
}