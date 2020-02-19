using System;

namespace Nez.Textures
{
    public sealed class IndexEventArgs : EventArgs
    {
        public int CurrentIndex 
        { 
            get;
            private set;
        }

        public int PreviousIndex
        {
            get;
            private set;
        }

        internal IndexEventArgs (int currentIndex, int previousIndex)
        {
            CurrentIndex = currentIndex;
            PreviousIndex = previousIndex;
        }
    }
}