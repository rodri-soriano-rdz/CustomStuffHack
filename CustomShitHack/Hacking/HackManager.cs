using DuckGame.CustomShitHack.ModInput;
using DuckGame.CustomShitHack.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Hacking
{
    internal static class HackManager
    {
        private const int DOUBLE_TAP_FRAMES = 10;

        private static readonly IDictionary<IHack, HackInfo> s_hacks = new Dictionary<IHack, HackInfo>();
        private static Keys s_lastKeyReleased = Keys.None;
        private static int s_lastReleaseTime = 0;
        private static bool s_doubleTap = false;

        public static void Initialize()
        {
            // Init hacks.
            s_hacks.Add(new HCameraManipulator(), new HackInfo("Camera Manipulation", Keys.LeftControl, HackKeyMode.HoldDoubleTapToggle));
            s_hacks.Add(new HThingManipulator(), new HackInfo("Thing Manipulation", Keys.LeftShift, HackKeyMode.HoldDoubleTapToggle));

            MainUpdater.OnUpdate += (s, a) => Update();
            MainUpdater.OnDraw += (object s, OnDrawEventArgs a) => OnDraw(a.Layer);

            ModKeyboard.OnKeyPressed += OnKeyPressed;
            ModKeyboard.OnKeyReleased += OnKeyReleased;
        }

        private static void Update()
        {
        }

        private static void OnDraw(Layer layer)
        {

        }

        private static void OnKeyPressed(object sender, KeyboardEventArgs a)
        {
            int tapDeltaTime = GameTime.FramesRunning - s_lastReleaseTime;

            if (tapDeltaTime < DOUBLE_TAP_FRAMES && a.Key == s_lastKeyReleased)
            {
                s_doubleTap = true;
            }

            for (int i = s_hacks.Count - 1; i >= 0; i--)
            {
                var kvp = s_hacks.ElementAt(i);

                HackInfo info = kvp.Value;

                if (a.Key == info.Keybind)
                {
                    if (info.Enabled)
                    {
                        if (info.Mode == HackKeyMode.Toggle) DisableHack(i);
                    }
                    else
                    {
                        EnableHack(i);
                    }
                }
            }
        }

        private static void OnKeyReleased(object sender, KeyboardEventArgs a)
        {
            for (int i = s_hacks.Count - 1; i >= 0; i--)
            {
                var kvp = s_hacks.ElementAt(i);

                HackInfo info = kvp.Value;

                if (a.Key == info.Keybind)
                {
                    switch (info.Mode)
                    {
                        case HackKeyMode.Hold:
                            DisableHack(i);
                        break;

                        case HackKeyMode.HoldDoubleTapToggle:
                            if (!s_doubleTap)
                            {
                                DisableHack(i);
                            }
                            else
                            {
                                SFX.Play("lowClick", 0.5f);
                            }
                            break;
                    }
                }
            }

            if (s_doubleTap)
            {
                s_doubleTap = false;
                s_lastKeyReleased = Keys.None;
                s_lastReleaseTime = 0;
            }
            else
            {
                s_lastKeyReleased = a.Key;
                s_lastReleaseTime = GameTime.FramesRunning;
            }
        }

        private static void EnableHack(int index)
        {
            if (index < 0 || index >= s_hacks.Count) return;

            var kvp = s_hacks.ElementAt(index);
            IHack hack = kvp.Key;
            HackInfo info = kvp.Value;

            if (info.Enabled) return;

            MainUpdater.OnUpdate += hack.OnUpdate;
            MainUpdater.OnDraw += hack.OnDraw;
            MainUpdater.OnPostUpdate += hack.OnPostUpdate;

            if (hack is IHackUsesMouse)
            {
                var mouseHack = hack as IHackUsesMouse;

                ModMouse.OnLeftClickDown += mouseHack.OnLeftClickDown;
                ModMouse.OnLeftClickPressed += mouseHack.OnLeftClickPressed;
                ModMouse.OnLeftClickReleased += mouseHack.OnLeftClickReleased;
                ModMouse.OnMiddleClickDown += mouseHack.OnMiddleClickDown;
                ModMouse.OnMiddleClickPressed += mouseHack.OnMiddleClickPressed;
                ModMouse.OnMiddleClickReleased += mouseHack.OnMiddleClickReleased;
                ModMouse.OnRightClickDown += mouseHack.OnRightClickDown;
                ModMouse.OnRightClickPressed += mouseHack.OnRightClickPressed;
                ModMouse.OnRightClickReleased += mouseHack.OnRightClickReleased;
                ModMouse.OnMouseScroll += mouseHack.OnMouseScroll;
                ModMouse.OnMouseMoved += mouseHack.OnMouseMoved;
                ModMouse.OnMouseMovedThroughWorld += mouseHack.OnMouseMovedThroughWorld;
            }

            if (hack is IHackUsesKeyboard)
            {
                var kbHack = hack as IHackUsesKeyboard;

                ModKeyboard.OnKeyDown += kbHack.OnKeyDown;
                ModKeyboard.OnKeyPressed += kbHack.OnKeyPressed;
                ModKeyboard.OnKeyReleased += kbHack.OnKeyReleased;
            }

            hack.OnEnable();
            s_hacks[hack].Enabled = true;

            SFX.Play("highClick", 0.5f);
        }

        private static void DisableHack(int index)
        {
            if (index < 0 || index >= s_hacks.Count) return;

            var kvp = s_hacks.ElementAt(index);
            IHack hack = kvp.Key;
            HackInfo info = kvp.Value;

            if (!info.Enabled) return;

            MainUpdater.OnUpdate -= hack.OnUpdate;
            MainUpdater.OnDraw -= hack.OnDraw;
            MainUpdater.OnPostUpdate -= hack.OnPostUpdate;

            if (hack is IHackUsesMouse)
            {
                var mouseHack = hack as IHackUsesMouse;

                ModMouse.OnLeftClickDown -= mouseHack.OnLeftClickDown;
                ModMouse.OnLeftClickPressed -= mouseHack.OnLeftClickPressed;
                ModMouse.OnLeftClickReleased -= mouseHack.OnLeftClickReleased;
                ModMouse.OnMiddleClickDown -= mouseHack.OnMiddleClickDown;
                ModMouse.OnMiddleClickPressed -= mouseHack.OnMiddleClickPressed;
                ModMouse.OnMiddleClickReleased -= mouseHack.OnMiddleClickReleased;
                ModMouse.OnRightClickDown -= mouseHack.OnRightClickDown;
                ModMouse.OnRightClickPressed -= mouseHack.OnRightClickPressed;
                ModMouse.OnRightClickReleased -= mouseHack.OnRightClickReleased;
                ModMouse.OnMouseScroll -= mouseHack.OnMouseScroll;
                ModMouse.OnMouseMoved -= mouseHack.OnMouseMoved;
                ModMouse.OnMouseMovedThroughWorld -= mouseHack.OnMouseMovedThroughWorld;
            }

            if (hack is IHackUsesKeyboard)
            {
                var kbHack = hack as IHackUsesKeyboard;

                ModKeyboard.OnKeyDown -= kbHack.OnKeyDown;
                ModKeyboard.OnKeyPressed -= kbHack.OnKeyPressed;
                ModKeyboard.OnKeyReleased -= kbHack.OnKeyReleased;
            }

            hack.OnDisable();
            s_hacks[hack].Enabled = false;

            SFX.Play("hatClosed", 0.5f);
        }
    }
}
