using DuckGame.CustomStuffHack.Images;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.ExtendedHats
{
    internal static class ExtendedHatsManager
    {
        private static IDictionary<TeamHat, BaseImage> s_extendedHats = new Dictionary<TeamHat, BaseImage>();

        public static void Initialize()
        {
            MainUpdater.OnPostUpdate += (s, a) => Update();
        }

        public static void AddHat(TeamHat hat, BaseImage image)
        {
            if (!s_extendedHats.ContainsKey(hat))
            {
                s_extendedHats.Add(hat, image);
                image.Initialize();
            }
        }

        public static void RemoveHat(TeamHat hat)
        {
            if (s_extendedHats.ContainsKey(hat))
            {
                s_extendedHats[hat].Terminate();
                s_extendedHats.Remove(hat);
                SFX.Play("demo");
            }
        }

        private static void Update()
        {
            foreach (var pair in s_extendedHats)
            {
                TeamHat hat = pair.Key;
                BaseImage image = pair.Value;

                if (hat == null || image == null) continue;

                image.Position = hat.position;
                image.Angle = hat.angle;
                image.FlipH = hat.offDir < 0;
            }
        }
    }
}
