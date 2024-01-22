using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.ModInput
{
    internal class KeyboardEventArgs : EventArgs
    {
        public Keys Key;
        public bool AltPressed;
        public bool ShiftPressed;
        public bool CtrlPressed;

        public KeyboardEventArgs(Keys key, bool altPressed, bool shiftPressed, bool ctrlPressed)
        {
            Key = key;
            AltPressed = altPressed;
            ShiftPressed = shiftPressed;
            CtrlPressed = ctrlPressed;
        }
    }
}
