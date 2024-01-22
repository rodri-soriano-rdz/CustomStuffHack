using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.ModInput;

namespace DuckGame.CustomShitHack.Hacking
{
    internal interface IHackUsesKeyboard : IHack
    {
        // Keyboard events
        void OnKeyPressed(object sender, KeyboardEventArgs args);
        void OnKeyDown(object sender, KeyboardEventArgs args);
        void OnKeyReleased(object sender, KeyboardEventArgs args);
    }
}
