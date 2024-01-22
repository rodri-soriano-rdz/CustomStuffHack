using DuckGame.CustomShitHack.Hacking;
using DuckGame.CustomShitHack.ModInput;
using DuckGame.CustomShitHack.Utility;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.Hacking
{
    internal class HThingManipulator : IHack, IHackUsesMouse, IHackUsesKeyboard
    {
        private const float MIN_SMOOTHING = 0.05f;
        private const float SMOOTHING_STEP = 0.1f;

        private PhysicsObject m_nearestObj;
        private bool m_isDragging;
        private bool m_locked;
        private bool m_positionDrag = false;
        private float m_smoothing = 1f;

        public void OnDraw(object sender, OnDrawEventArgs args)
        {
            if (args.Layer != Layer.Console) return;
            if (m_nearestObj == null) return;

            Vec2 objConsolePos = PosTransform.ToConsolePos(m_nearestObj.position, m_nearestObj.layer);
            Vec2 mousePos = ModMouse.PosConsole;

            Graphics.DrawLine(mousePos, objConsolePos, Color.White, 1f);

            BitmapFont font = FontLoader.GetFontOrDefault("smallBios");
            font.DrawOutline(m_nearestObj.editorName, ModMouse.PosConsole + new Vec2(10f), Color.White, Color.Black, 1f);
        }

        public void OnUpdate(object sender, EventArgs args)
        {
            if (m_nearestObj == null) return;

            if (m_nearestObj.removeFromLevel
                || m_nearestObj.destroyed
                || !m_nearestObj.active)
            {
                Deselect();
                return;
            }

            if (!m_isDragging) return;

            Thing.PowerfulRuleBreakingFondle(m_nearestObj, DuckNetwork.localConnection);

            if (m_positionDrag)
            {
                m_nearestObj.position = ModMouse.PosWorld;
            }
            else
            {
                m_nearestObj.velocity = ModMouse.PosWorld - m_nearestObj.position;
                m_nearestObj.velocity *= m_smoothing - MIN_SMOOTHING;
            }
        }

        public void OnPostUpdate(object sender, EventArgs args) { }

        public void OnEnable()
        {
            m_isDragging = false;

            if (!m_locked)
            {
                m_nearestObj = Level.Nearest<PhysicsObject>(ModMouse.PosWorld);
            }
        }

        public void OnDisable() { }

        public void OnLeftClickDown(object sender, MouseEventArgs args) { }

        public void OnLeftClickPressed(object sender, MouseEventArgs args)
        {
            m_isDragging = true;
        }

        public void OnLeftClickReleased(object sender, MouseEventArgs args)
        {
            m_isDragging = false;
        }

        public void OnMiddleClickDown(object sender, MouseEventArgs args) { }

        public void OnMiddleClickPressed(object sender, MouseEventArgs args)
        {
            SFX.Play("snubbyLoad", 0.5f, m_locked ? 0f : 0.3f);
            m_locked = !m_locked;
        }

        public void OnMiddleClickReleased(object sender, MouseEventArgs args) { }

        public void OnMouseMoved(object sender, MouseEventArgs args) { }

        public void OnMouseMovedThroughWorld(object sender, MouseEventArgs args)
        {
            if (m_locked) return;
            if (m_isDragging) return;

            int index = 0;
            bool stop;

            do
            {
                m_nearestObj = Level.Nearest<PhysicsObject>(args.MousePositionWorld, null, index);

                stop = true;

                if (m_nearestObj is TeamHat)
                {
                    var teamHat = (TeamHat)m_nearestObj;

                    if (teamHat.IsImageTeamHat() || teamHat.owner != null)
                    {
                        stop = false;
                    }
                }

                index++;
            }
            while (!stop);
        }

        public void OnMouseScroll(object sender, MouseScrollEventArgs args)
        { 
            if (args.ScrollDirection == ScrollDirection.Up)
            {
                m_smoothing *= 1f + SMOOTHING_STEP;
            }
            else
            {
                m_smoothing *= 1f - SMOOTHING_STEP;
            }

            m_smoothing = Maths.Clamp(m_smoothing, MIN_SMOOTHING * 2, 1f + MIN_SMOOTHING);
        }

        public void OnRightClickDown(object sender, MouseEventArgs args) { }

        public void OnRightClickPressed(object sender, MouseEventArgs args)
        { 
            if (m_nearestObj != null)
            {
                OnRightClick(m_nearestObj);
            }
        }

        public void OnRightClickReleased(object sender, MouseEventArgs args) { }

        public void OnKeyPressed(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.LeftAlt)
            {
                m_positionDrag = true;
            }
        }

        public void OnKeyDown(object sender, KeyboardEventArgs args)
        {
            
        }

        public void OnKeyReleased(object sender, KeyboardEventArgs args)
        {
            if (args.Key == Keys.LeftAlt)
            {
                m_positionDrag = false;
            }
        }

        public void OnRightClick(PhysicsObject obj)
        {
            Thing.PowerfulRuleBreakingFondle(m_nearestObj, DuckNetwork.localConnection);

            if (obj is Duck)
            {
                var duck = (Duck)obj;

                if (duck.ragdoll == null)
                {
                    duck.GoRagdoll();
                    m_nearestObj = duck.ragdoll.part2;
                }
            }
            else if (obj is Gun)
            {
                var gun = (Gun)obj;
                gun.PressAction();
            }
        }

        private void Deselect()
        {
            m_nearestObj = null;
            m_locked = false;
            m_isDragging = false;
        }
    }
}
