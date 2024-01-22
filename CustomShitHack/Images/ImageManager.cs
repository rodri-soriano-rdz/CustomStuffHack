using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomStuffHack.Utility;

namespace DuckGame.CustomStuffHack.Images
{
    /// <summary>
    /// Manages BaseImages.
    /// </summary>
    internal static class ImageManager
    {
        private const int FRAMES_PER_UPDATE = 20;

        private static readonly IList<BaseImage> s_activeImages = new List<BaseImage>();

        public static BaseImage[] ActiveImages => s_activeImages.ToArray();

        public static void Initialize()
        {
            MainUpdater.OnUpdate += (s, a) => Update();
            MainUpdater.OnDraw += (object s, OnDrawEventArgs args) => OnDraw(args.Layer);
        }

        /// <summary>
        /// Keeps an image updated.
        /// </summary>
        public static void StartUpdatingImage(BaseImage img)
        {
            if (!s_activeImages.Contains(img))
            {
                s_activeImages.Add(img);
                img.Sync();
                Logger.Log($"Began updating image'!", LogSeverity.Information);
            }
        }

        /// <summary>
        /// Stops updating an image.
        /// </summary>
        public static void StopUpdatingImage(BaseImage img)
        {
            if (s_activeImages.Contains(img))
            {
                s_activeImages.Remove(img);
                Logger.Log($"Stopped updating image!", LogSeverity.Information);
            }
        }

        private static void Update()
        {
            if (Level.current is RockScoreboard)
            {
                var lvl = Level.current as RockScoreboard;

                if (lvl.mode == ScoreBoardMode.ShowScores)
                {
                    return;
                }
            }

            TeamSynchronizer.PerformSynchronizations();

            bool timeForImageUpdate = GameTime.FramesRunning % FRAMES_PER_UPDATE == 0;

            // Update images.
            if (timeForImageUpdate)
            {
                for (int i = s_activeImages.Count - 1; i >= 0; i--)
                {
                    s_activeImages[i].Update();
                    s_activeImages[i].UpdateOptimized();
                }
            }
            else
            {
                for (int i = s_activeImages.Count - 1; i >= 0; i--)
                {
                    s_activeImages[i].Update();
                }
            }
        }

        private static void OnDraw(Layer layer)
        {
            if (Level.current is RockScoreboard)
            {
                var lvl = Level.current as RockScoreboard;

                if (lvl.mode == ScoreBoardMode.ShowScores)
                {
                    return;
                }
            }

            // TODO: Image editing with mouse
        }
    }
}
