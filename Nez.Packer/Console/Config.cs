
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Nez.Tools.Packing
{
    public class Config
    {
        /// <summary>
        /// 
        /// </summary>
        public static string[] Read(string[] arguments)
        {        
            var configPath = arguments?[0];
            return Read(configPath);
        }

        /// <summary>
        /// Formats and checks each line and returns an array of .config arguments.
        /// </summary>
        public static string[] Read (string configPath)
        {
            //Check if the string is empty
            if (configPath == null || configPath == "")
                return null;

            //Check if the path is valid
            if (!File.Exists(configPath) || Path.GetExtension(configPath) != ".config")
                return null;

            var config = new List<string>();

            StreamReader reader = new StreamReader(configPath);
            string argument;

            while ((argument = reader.ReadLine()) != null)
            {
                //Ignore blank lines and lines without delimiters
                if (string.IsNullOrWhiteSpace(argument)) continue;
                if (argument.IndexOfAny(new char[] { ':', '=' }, 1) == -1) continue;

                //Remove any whitespace and make all lowercase
                argument = argument.ToLowerInvariant();
                argument = argument.Replace(" ", string.Empty);

                //Ignore comments or invalid arguments
                if (!char.IsLetter(argument[0])) continue;

                config.Add(argument);
            }
            return config.ToArray();

        }
    }
}