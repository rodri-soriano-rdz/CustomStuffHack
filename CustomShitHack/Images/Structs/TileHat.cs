using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.Images
{
    internal readonly struct TileHat
    {
        public readonly TeamHat TeamHat;
        public readonly Vec2 Coordinates;

        public TileHat(TeamHat teamHat, Vec2 tilePosition)
        {
            TeamHat = teamHat;
            Coordinates = tilePosition;
        }
    }
}
