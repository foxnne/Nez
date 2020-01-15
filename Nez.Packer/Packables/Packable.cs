using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Nez.Tools.Packing
{
    ///<summary>
    /// Contains base configurable fields and packing logic.
    ///</summary>
    public abstract class Packable<T> where T : Packable<T>
    {
        /// <summary>
        /// The path of the final packed output image, relative to the project root.
        /// </summary>
        [Argument(ArgumentType.Required, ShortName = "", HelpText = "Output file name for the image.")]
        public string outputimage;

        /// <summary>
        /// The path of the final output map, relative to the project root.
        /// <para>
        /// This string is used to try to match a config with a Packable.
        /// If this is null/empty after parsing to the BasePackable, the
        /// DefaultPackable will be used.
        /// </para>
        /// </summary>
        [Argument(ArgumentType.AtMostOnce, ShortName = "", HelpText = "Output file name for the map.")]
        public string outputmap;

        /// <summary>
        /// Possible extensions of outputmap.
        /// <para> 
        /// Any derived class that returns one or more extensions
        /// will be searched for matches with a loaded config
        /// and Pack() will be called via reflection.
        /// </para>
        /// </summary>
        public virtual string[] mapExtensions
        {
            get => new string[] { };
        }

        /// <summary>
        /// Parses the .config arguments into the derived class.
        /// </summary>
        /// 
        /// <param name = "config">
        /// Arguments from the config file.
        /// </param>
        public T Parse(params string[] config)
        {
            if (Parser.ParseArgumentsWithUsage(config, (T)this))
                return (T)this;
            return null;
        }

        /// <summary>
        /// Override this to implement the packing logic.
        /// </summary>
        /// 
        /// <param name = "path">
        /// Root path to begin searching for images.
        /// </param>
        public virtual int Pack(string path) { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Concrete implementation of abstract Packable&lt;&gt; used to select and pack the correct packable via reflection.
    /// </summary>
    public class Packable : Packable<Packable>
    {
        /// <summary>
        /// Determines which derived class to use based on mapOutput and calls Pack() on the resulting class.
        /// </summary>
        public static int Pack(string configPath, string[] config)
        {
            //Parse to an instance of this class for only the output image and type
            var packable = new Packable().Parse(config);
            if (packable == null) return (int)FailCode.FailedParsingConfig;

            //Isolate the desired outputmap extension
            var mapExtension = Path.GetExtension(packable.outputmap);

            //Get all classes implementing Packable<T>
            var packables = from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.IsClass &&
                                  t.BaseType != null && t.BaseType.IsGenericType &&
                                  t.BaseType.GetGenericTypeDefinition() == typeof(Packable<>)
                            select t;

            //Determine if any instances have a matching extension defined
            foreach (var p in packables)
            {
                var instance = Activator.CreateInstance(p);
                var extensions = (string[])p.GetProperty("mapExtensions").GetMethod.Invoke(instance, null);
                if (extensions.Contains(mapExtension))
                {
                    //Match found, pack using this packer
                    if (Parser.ParseArgumentsWithUsage(config, instance))
                        return (int)p.GetMethod("Pack").Invoke(instance, new object[] { configPath });
                }
            }

            //No matching instances found, run the default packer
            var defaultPackable = new DefaultPackable().Parse(config);
            if (defaultPackable == null) return (int)FailCode.FailedParsingConfig;

            return defaultPackable.Pack(configPath);
        }
    }
}

