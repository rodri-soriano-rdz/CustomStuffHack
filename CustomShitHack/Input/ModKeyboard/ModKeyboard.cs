using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.ModInput
{
    internal static class ModKeyboard
    {
        public static EventHandler<KeyboardEventArgs> OnKeyPressed;
        public static EventHandler<KeyboardEventArgs> OnKeyDown;
        public static EventHandler<KeyboardEventArgs> OnKeyReleased;

        public static void Initialize()
        {
            MainUpdater.OnUpdate += (s, e) => Update();
        }

        private static void Update()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.None) continue;

                if (Keyboard.Pressed(key))
                {
                    var args = new KeyboardEventArgs(key, Keyboard.alt, Keyboard.shift, Keyboard.control);
                    OnKeyPressed?.Invoke(new object(), args);
                }

                if (Keyboard.Down(key))
                {
                    var args = new KeyboardEventArgs(key, Keyboard.alt, Keyboard.shift, Keyboard.control);
                    OnKeyDown?.Invoke(new object(), args);
                }

                if (Keyboard.Released(key))
                {
                    var args = new KeyboardEventArgs(key, Keyboard.alt, Keyboard.shift, Keyboard.control);
                    OnKeyReleased?.Invoke(new object(), args);
                }
            }
        }
    }
}
