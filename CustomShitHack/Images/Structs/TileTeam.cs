using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Images
{
    internal readonly struct TileTeam
    {
        public readonly Team Team;
        public readonly Vec2 Coordinates;

        public TileTeam(Team team, Vec2 tilePosition)
        {
            Team = team;
            Coordinates = tilePosition;
        }
    }
}
