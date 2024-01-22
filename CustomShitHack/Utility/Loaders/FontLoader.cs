using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DuckGame.CustomShitHack.Utility
{
    internal static class FontLoader
    {
        private static readonly IDictionary<string, BitmapFont> s_loadedFonts = new Dictionary<string, BitmapFont>();

        public static bool TryLoadFont(string textureName, int width, int height, string fontName)
        {
            if (s_loadedFonts.ContainsKey(fontName))
            {
                return false;
            }

            string path = CSH_Mod.GetContentPath($"textures/fonts/{textureName}");

            s_loadedFonts.Add(fontName, new BitmapFont(path, width, height));
            return true;
        }

        public static bool TryGetFont(string fontName, out BitmapFont font)
        {
            return s_loadedFonts.TryGetValue(fontName, out font);
        }

        public static BitmapFont GetFontOrDefault(string fontName)
        {
            BitmapFont font;
            bool found = s_loadedFonts.TryGetValue(fontName, out font);

            // Fallback font.
            if (!found)
            {
                font = Graphics._biosFont;
            }

            return font;
        }
    }
}
