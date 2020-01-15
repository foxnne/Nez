using System;

namespace Nez.Tools.Packing
{
    public class DefaultPackable : Packable<DefaultPackable>
    {
        public override string[] mapExtensions => base.mapExtensions;

        public override int Pack(string path)
        {
            throw new NotImplementedException();
        }

    }

}
