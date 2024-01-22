using DuckGame.CustomShitHack.ModInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Hacking
{
    internal interface IHackUsesMouse
    {
        // Mouse movement events
        void OnMouseMoved(object sender, MouseEventArgs args);
        void OnMouseMovedThroughWorld(object sender, MouseEventArgs args);

        // Mouse click events
        void OnLeftClickPressed(object sender, MouseEventArgs args);
        void OnRightClickPressed(object sender, MouseEventArgs args);
        void OnMiddleClickPressed(object sender, MouseEventArgs args);

        void OnLeftClickDown(object sender, MouseEventArgs args);
        void OnRightClickDown(object sender, MouseEventArgs args);
        void OnMiddleClickDown(object sender, MouseEventArgs args);

        void OnLeftClickReleased(object sender, MouseEventArgs args);
        void OnRightClickReleased(object sender, MouseEventArgs args);
        void OnMiddleClickReleased(object sender, MouseEventArgs args);

        // Mouse scroll events
        void OnMouseScroll(object sender, MouseScrollEventArgs args);
    }
}
