using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Hacking
{
    internal class HackInfo
    {
        public readonly string Name;
        public readonly Keys Keybind;
        public readonly HackKeyMode Mode;
        public bool Enabled;

        public HackInfo(string name, Keys keybind = Keys.None, HackKeyMode mode = HackKeyMode.Toggle, bool enabled = false)
        {
            Name = name;
            Keybind = keybind;
            Mode = mode;
            Enabled = enabled;
        }
    }
}
