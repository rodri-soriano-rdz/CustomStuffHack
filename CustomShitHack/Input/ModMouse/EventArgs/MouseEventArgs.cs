using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.ModInput
{
    internal class MouseEventArgs : EventArgs
    {
        public readonly Vec2 MousePosition;
        public readonly Vec2 MousePositionWindow;
        public readonly Vec2 MousePositionWorld;
        public readonly Vec2 MousePositionConsole;

        public MouseEventArgs(Vec2 position, Vec2 posWindow, Vec2 posWorld, Vec2 posConsole)
        {
            MousePosition = position;
            MousePositionWindow = posWindow;
            MousePositionWorld = posWorld;
            MousePositionConsole = posConsole;
        }
    }
}
