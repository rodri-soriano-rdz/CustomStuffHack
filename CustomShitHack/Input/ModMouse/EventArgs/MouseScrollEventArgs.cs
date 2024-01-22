using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.ModInput
{
    internal class MouseScrollEventArgs : EventArgs
    {
        public readonly ScrollDirection ScrollDirection;
        public readonly Vec2 MousePosition;
        public readonly Vec2 MousePositionWindow;
        public readonly Vec2 MousePositionWorld;
        public readonly Vec2 MousePositionConsole;

        public MouseScrollEventArgs(ScrollDirection direction, Vec2 position, Vec2 posWindow, Vec2 posWorld, Vec2 posConsole)
        {
            ScrollDirection = direction;
            MousePosition = position;
            MousePositionWindow = posWindow;
            MousePositionWorld = posWorld;
            MousePositionConsole = posConsole;
        }
    }
}
