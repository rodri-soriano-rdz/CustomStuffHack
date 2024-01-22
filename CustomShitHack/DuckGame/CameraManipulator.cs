//using DuckGame.CustomShitHack.UI.ModMouse;
//using Harmony;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace DuckGame.CustomShitHack.DuckGame
//{
//    internal class CameraManipulator
//    {
//        private const float ZOOM_SENSITIVITY = 0.1f;

//        /// <summary>
//        /// Current level's original camera.
//        /// </summary>
//        public static Camera OriginalCamera;
//        private Vec2 m_mouseClickPos;
//        private Vec2 m_prevCamPos;
//        private float m_zoom;
//        private bool m_movingCamera = false;

//        /// <summary>
//        /// Current level's camera.
//        /// </summary>
//        public Camera LevelCamera
//        {
//            get
//            {
//                return Level.current.camera;
//            }
//            set
//            {
//                Level.current.camera = value;
//            }
//        }

//        public string Name => "Camera Mode";

//        public void Update()
//        {
//            m_zoom = LevelCamera.size.x / Resolution.size.x;
//            CustomMouse.SetCursor(MouseCursor.Move);
//        }

//        public void Draw(Layer l)
//        {
//        }

//        public void OnMouseMove()
//        {
//            // Move camera around.
//            if (m_movingCamera)
//            {
//                Vec2 deltaPos = m_mouseClickPos - CustomMouse.Position * m_zoom;
//                Vec2 newPos = m_prevCamPos + deltaPos;

//                LevelCamera.position = newPos;
//            }
//        }

//        public void OnButtonPressed(MouseButton button)
//        {
//            // Reset camera.
//            if (button == MouseButton.RightClick && !m_movingCamera)
//            {
//                ResetCamera();
//            }

//            // Start moving camera.
//            if (button == MouseButton.LeftClick)
//            {
//                m_mouseClickPos = CustomMouse.Position * m_zoom;
//                m_prevCamPos = LevelCamera.position;
//                m_movingCamera = true;

//                UnlockCamera();
//            }
//        }

//        public void OnButtonReleased(MouseButton button)
//        {
//            // Stop moving camera.
//            if (button == MouseButton.LeftClick)
//            {
//                m_movingCamera = false;
//            }
//        }

//        public void OnButtonDown(MouseButton button)
//        {

//        }

//        public void ZoomIn(float zoomMultiplier = 1.0f)
//        {
//            // Zoom in-out.
//            UnlockCamera();

//            // Store position before zoom change.
//            float zoomMult = 1f * zoomMultiplier * ZOOM_SENSITIVITY;

//            // Perform zooming.
//            LevelCamera.size *= zoomMult;
//        }

//        /// <summary>
//        /// Unlocks camera to move it freely.
//        /// </summary>
//        private void UnlockCamera()
//        {
//            if (LevelCamera is FollowCam)
//            {
//                AccessTools.Field(typeof(FollowCam), "_skipResize").SetValue(LevelCamera as FollowCam, true);
//            }
//            else if (LevelCamera.Equals(OriginalCamera))
//            {
//                LevelCamera = new Camera();

//                foreach (FieldInfo f in OriginalCamera.GetType().GetFields())
//                {
//                    if (!f.IsInitOnly) continue;

//                    f.SetValue(LevelCamera, f.GetValue(OriginalCamera));
//                }

//                foreach (PropertyInfo p in OriginalCamera.GetType().GetProperties())
//                {
//                    if (!p.CanWrite) continue;

//                    p.SetValue(LevelCamera, p.GetValue(OriginalCamera));
//                }
//            }
//        }

//        /// <summary>
//        /// Returns camera to it's original state.
//        /// </summary>
//        private void ResetCamera()
//        {
//            if (LevelCamera is FollowCam)
//            {
//                AccessTools.Field(typeof(FollowCam), "_skipResize").SetValue(LevelCamera, false);
//                AccessTools.Field(typeof(FollowCam), "_checkedZoom").SetValue(LevelCamera, false);
//            }
//            else if (!LevelCamera.Equals(OriginalCamera))
//            {
//                LevelCamera = OriginalCamera;
//            }
//        }
//    }
//}
