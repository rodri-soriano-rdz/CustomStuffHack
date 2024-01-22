using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack
{
    internal class OnDrawEventArgs : EventArgs
    {
        public readonly Layer Layer;

        public OnDrawEventArgs(Layer layer)
        {
            Layer = layer;
        }
    }
}
