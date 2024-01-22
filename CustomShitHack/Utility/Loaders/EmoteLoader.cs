using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Harmony;

namespace DuckGame.CustomShitHack.Utility
{
    internal static class EmoteLoader
    {
        private static IDictionary<string, Sprite> s_emotes;

        static EmoteLoader()
        {
            try
            {
                s_emotes = AccessTools.Field(typeof(Input), "_triggerImageMap").GetValue(null) as Dictionary<string, Sprite>;
            }
            catch (Exception e)
            {
            }
        }

        public static bool TryLoad(string name, Sprite spr)
        {
            if (s_emotes == null)
            {
                return false;
            }

            if (s_emotes.ContainsKey(name))
            {
                return false;
            }

            s_emotes.Add(name, spr);
            return true;
        }
    }
}
