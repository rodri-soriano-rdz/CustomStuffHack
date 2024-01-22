using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.Utility;

namespace DuckGame.CustomShitHack.Images
{
    internal readonly struct TeamImage
    {
        public readonly TileTeam[] Teams;
        public readonly Vec2 TotalTiles;
        public readonly Vec2 ImageSize;

        public TeamImage(TileTeam[] teams, Vec2 totalTiles, Vec2 imageSize)
        {
            Teams = teams;
            TotalTiles = totalTiles;
            ImageSize = imageSize;
        }
    }
}
