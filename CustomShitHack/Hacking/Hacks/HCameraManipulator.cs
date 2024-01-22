using DuckGame.CustomStuffHack.Hacking;
using DuckGame.CustomStuffHack.ModInput;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.Hacking
{
    internal class HCameraManipulator : IHack, IHackUsesMouse
    {
        private const float ZOOM_SENSIBILITY = 0.1f;

        private static bool s_cameraUnlocked = false;
        private static Camera s_originalCamera;
        private bool m_movingCamera = false;
        private Vec2 m_mouseClickPos;
        private Vec2 m_prevCamPos;
        private float m_zoom;

        public string Name => "Cam Manipulator";
        public string Description => "Allows you to manipulate the camera with your mouse.";

        /// <summary>
        /// Current level's camera.
        /// </summary>
        public static Camera LevelCamera
        {
            get
            {
                return Level.current.camera;
            }
            set
            {
                Level.current.camera = value;
            }
        }

        public void OnDraw(object sender, OnDrawEventArgs args)
        {
            
        }

        public void OnUpdate(object sender, EventArgs args)
        {
            m_zoom = LevelCamera.size.x / Resolution.size.x;
        }

        public void OnPostUpdate(object sender, EventArgs args) { }

        public void OnEnable() { }

        public void OnDisable()
        {
            m_movingCamera = false;
        }

        public void OnLeftClickDown(object sender, MouseEventArgs args) {}

        public void OnLeftClickPressed(object sender, MouseEventArgs args)
        {
            UnlockCamera();

            m_mouseClickPos = ModMouse.Position * m_zoom;
            m_prevCamPos = LevelCamera.position;
            m_movingCamera = true;
        }

        public void OnLeftClickReleased(object sender, MouseEventArgs args)
        {
            m_movingCamera = false;
        }

        public void OnMiddleClickDown(object sender, MouseEventArgs args) { }

        public void OnMiddleClickPressed(object sender, MouseEventArgs args) { }

        public void OnMiddleClickReleased(object sender, MouseEventArgs args) { }

        public void OnMouseMoved(object sender, MouseEventArgs args)
        {
            // Move camera around.
            if (m_movingCamera)
            {
                Vec2 deltaPos = m_mouseClickPos - args.MousePosition * m_zoom;
                Vec2 newPos = m_prevCamPos + deltaPos;

                LevelCamera.position = newPos;
            }
        }

        public void OnMouseMovedThroughWorld(object sender, MouseEventArgs args) { }

        public void OnMouseScroll(object sender, MouseScrollEventArgs args)
        {
            // Zoom in-out.
            UnlockCamera();

            // Store position before zoom change.
            Vec2 prevPos = ModMouse.PosWorld;
            float zoomMult = 1 + ModMouse.ScrollNormalized * ZOOM_SENSIBILITY;

            // Perform zooming.
            LevelCamera.size *= zoomMult;

            // Get difference between positions before and after zoom.
            Vec2 pos = ModMouse.PosWorld;
            Vec2 posDelta = prevPos - pos;

            // Maintain camera zooming anchored to the mouse.
            LevelCamera.position += posDelta;
        }

        public void OnRightClickDown(object sender, MouseEventArgs args) { }

        public void OnRightClickPressed(object sender, MouseEventArgs args)
        {
            //Reset camera.
            ResetCamera();

            m_movingCamera = false;
        }

        public void OnRightClickReleased(object sender, MouseEventArgs args) { }

        public static void OnLevelChange(Level newLevel)
        {
            s_originalCamera = newLevel.camera;
            s_cameraUnlocked = false;
        }

        /// <summary>
        /// Unlocks camera to move it freely.
        /// </summary>
        private static void UnlockCamera()
        {
            if (s_cameraUnlocked)
            {
                return;
            }

            s_cameraUnlocked = true;

            if (LevelCamera is FollowCam)
            {
                AccessTools.Field(typeof(FollowCam), "_skipResize").SetValue(LevelCamera as FollowCam, true);
            }
            else if (LevelCamera.Equals(s_originalCamera))
            {
                LevelCamera = new Camera();

                foreach (FieldInfo f in s_originalCamera.GetType().GetFields())
                {
                    if (!f.IsInitOnly) continue;

                    f.SetValue(LevelCamera, f.GetValue(s_originalCamera));
                }

                foreach (PropertyInfo p in s_originalCamera.GetType().GetProperties())
                {
                    if (!p.CanWrite) continue;

                    p.SetValue(LevelCamera, p.GetValue(s_originalCamera));
                }
            }
        }

        /// <summary>
        /// Returns camera to it's original state.
        /// </summary>
        private static void ResetCamera()
        {
            if (!s_cameraUnlocked)
            {
                return;
            }

            s_cameraUnlocked = false;

            if (LevelCamera is FollowCam)
            {
                AccessTools.Field(typeof(FollowCam), "_skipResize").SetValue(LevelCamera, false);
                AccessTools.Field(typeof(FollowCam), "_checkedZoom").SetValue(LevelCamera, false);
            }
            else if (!LevelCamera.Equals(s_originalCamera))
            {
                LevelCamera = s_originalCamera;
            }
        }
    }
}
