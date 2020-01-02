using System;


namespace Nez.Tools.Packing
{
    ///<summary>
    ///
    ///</summary>
    public abstract class Packable<T> where T : Packable<T>
    {
        [Argument(ArgumentType.Required, ShortName = "", HelpText = "Output file name for the image.")]
        public string outputimage;

        [Argument(ArgumentType.Required, ShortName = "", HelpText = "Output file name for the map.")]
        public string outputmap;

        public virtual string[] imageExtensions
        {
            get => new string[] { ".png", ".jpeg", ".bmp" };
        }

        public virtual string[] mapExtensions
        {
            get => new string[] { };
        }

        public T Parse(params string[] args)
        {
            if (Parser.ParseArgumentsWithUsage(args, (T)this))
                return (T)this;
            return null;
        }
        public virtual int Pack(string path) { throw new NotImplementedException(); }
    }

    //Implement abstract class so we can use it to get just the output image and map
    public class BasePackable : Packable<BasePackable> { }

}

