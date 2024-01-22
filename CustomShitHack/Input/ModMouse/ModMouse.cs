using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomStuffHack.Utility;
using DuckGame.CustomStuffHack.UI;

namespace DuckGame.CustomStuffHack.ModInput
{
    internal static class ModMouse
    {
        private const int STICK_SENSIBILITY = 8;
        private static readonly SpriteMap s_mouseSpr = new SpriteMap(CSH_Mod.GetContentPath("textures/mouse/cursors"), 14, 14, true)
        {
            depth = DepthTable.MOUSE
        };

        private static MouseCursor s_cursor = MouseCursor.Idle;

        private static bool s_movedThisFrame = false;
        private static Vec2 s_prevPos;

        private static bool s_movedThroughWorldThisFrame = false;
        private static Vec2 s_prevWorldPos;

        public static EventHandler<MouseEventArgs> OnMouseMoved;
        public static EventHandler<MouseEventArgs> OnMouseMovedThroughWorld;

        public static EventHandler<MouseEventArgs> OnLeftClickPressed;
        public static EventHandler<MouseEventArgs> OnRightClickPressed;
        public static EventHandler<MouseEventArgs> OnMiddleClickPressed;

        public static EventHandler<MouseEventArgs> OnLeftClickDown;
        public static EventHandler<MouseEventArgs> OnRightClickDown;
        public static EventHandler<MouseEventArgs> OnMiddleClickDown;

        public static EventHandler<MouseEventArgs> OnLeftClickReleased;
        public static EventHandler<MouseEventArgs> OnRightClickReleased;
        public static EventHandler<MouseEventArgs> OnMiddleClickReleased;

        public static EventHandler<MouseScrollEventArgs> OnMouseScroll;

        public static InputState Left => Mouse.left;
        public static InputState Middle => Mouse.middle;
        public static InputState Right => Mouse.right;

        public static bool MovedThisFrame => s_movedThisFrame;
        public static bool MovedThroughWorldThisFrame => s_movedThroughWorldThisFrame;
        public static Vec2 Position => Mouse.mousePos;
        public static Vec2 PosWindow => Mouse.position;
        public static Vec2 PosConsole => Mouse.positionConsole;
        public static Vec2 PosWorld => Mouse.positionScreen;    
        public static float Scroll => Mouse.scroll;
        public static bool Scrolled => Mouse.scroll != 0f;
        public static float ScrollNormalized => Math.Sign(Scroll);

        public static void Initialize()
        {
            MainUpdater.OnUpdate += (s, a) => Update();
            MainUpdater.OnPostUpdate += (s, a) => PostUpdate();
            MainUpdater.OnDraw += (object s, OnDrawEventArgs args) => OnDraw(args.Layer);
        }

        public static void SetCursor(MouseCursor cursor)
        {
            s_cursor = cursor;
        }

        public static void SetPosition(float x, float y)
        {
            Mouse.position = new Vec2(x, y);
        }

        public static void SetPosition(Vec2 position)
        {
            Mouse.position = position;
        }

        public static void SetX(float posX)
        {
            Mouse.position = new Vec2(posX, Mouse.position.y);
        }

        public static void SetY(float posY)
        {
            Mouse.position = new Vec2(Mouse.position.x, posY);
        }

        public static void AddX(float amount)
        {
            Mouse.position = new Vec2(Mouse.position.x + amount, Mouse.position.y);
        }

        public static void AddY(float amount)
        {
            Mouse.position = new Vec2(Mouse.position.x, Mouse.position.y + amount);
        }

        private static void Update()
        {
            // Detect mouse movement.
            if (s_prevPos != Position)
            {
                s_movedThisFrame = true;

                // Invoke event.
                OnMouseMoved?.Invoke(new object(), GetMouseEventArgs());
            }
            else
            {
                s_movedThisFrame = false;
            }

            // Detect mouse world movement.
            if (s_prevWorldPos != PosWorld)
            {
                s_movedThroughWorldThisFrame = true;

                // Invoke event.
                OnMouseMovedThroughWorld?.Invoke(new object(), GetMouseEventArgs());
            }
            else
            {
                s_movedThroughWorldThisFrame = false;
            }

            // Click pressed.
            if (Left == InputState.Pressed)
            {
                OnLeftClickPressed?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Right == InputState.Pressed)
            {
                OnRightClickPressed?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Middle == InputState.Pressed)
            {
                OnMiddleClickPressed?.Invoke(new object(), GetMouseEventArgs());
            }

            // Click released.
            if (Left == InputState.Released)
            {
                OnLeftClickReleased?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Right == InputState.Released)
            {
                OnRightClickReleased?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Middle == InputState.Released)
            {
                OnMiddleClickReleased?.Invoke(new object(), GetMouseEventArgs());
            }

            // Click held down.
            if (Left == InputState.Down)
            {
                OnLeftClickDown?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Right == InputState.Down)
            {
                OnRightClickDown?.Invoke(new object(), GetMouseEventArgs());
            }
            if (Middle == InputState.Down)
            {
                OnMiddleClickDown?.Invoke(new object(), GetMouseEventArgs());
            }

            // Detect mouse scrolling.
            if (Scrolled)
            {
                ScrollDirection direction;

                if (Scroll > 0f)
                {
                    direction = ScrollDirection.Down;
                }
                else
                {
                    direction = ScrollDirection.Up;
                }

                OnMouseScroll?.Invoke(new object(), new MouseScrollEventArgs(direction, Position, PosWindow, PosWorld, PosConsole));
            }
        }

        private static void PostUpdate()
        {
            // Send current position to next update loop.
            s_prevPos = Position;
            s_prevWorldPos = PosWorld;
        }

        private static void OnDraw(Layer layer)
        {
            if (layer != Layer.Console) return;

            // Get cursor spritemap frame.
            int cursorFrame;

            switch(s_cursor)
            {
                case MouseCursor.Idle:
                default:
                    cursorFrame = 0;
                    break;
                case MouseCursor.Select:
                    cursorFrame = 1;
                    break;
                case MouseCursor.Grab:
                    cursorFrame = 2;
                    break;
                case MouseCursor.Move:
                    cursorFrame = 3;
                    break;
                case MouseCursor.Type:
                    cursorFrame = 4;
                    break;
            }

            // Draw cursor.
            Graphics.Draw(s_mouseSpr, cursorFrame, PosConsole.x, PosConsole.y, 1f, 1f, false);
        }

        private static MouseEventArgs GetMouseEventArgs()
        {
            return new MouseEventArgs(Position, PosWindow, PosWorld, PosConsole);
        }
    }
}
