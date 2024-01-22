using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Utility
{
    internal static class GameTime
    {
        private static readonly Timer m_timer = new Timer();
        private static int m_framesRunning = 0;
        private static float m_framerate = 0;
        private static float m_deltaTime = 0;

        public static int FramesRunning => m_framesRunning;
        public static float Framerate => m_framerate;
        public static float DeltaTime => m_deltaTime;

        public static void Initialize()
        {
            MainUpdater.OnUpdate += (s, a) => OnUpdate();
            MainUpdater.OnDraw += (object s, OnDrawEventArgs args) => OnDraw(args.Layer);
        }

        private static void OnDraw(Layer layer)
        {
            if (layer != Layer.Game) return;

            m_deltaTime = (float)m_timer.elapsed.TotalSeconds;
            m_framerate = 1 / (float)m_deltaTime;

            m_timer.Restart();
        }

        private static void OnUpdate()
        {
            m_framesRunning++;
        }
    }
}
