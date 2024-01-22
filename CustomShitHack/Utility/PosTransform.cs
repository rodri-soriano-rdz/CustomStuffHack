using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack
{
    /// <summary>
    /// Tool methods for transforming positions from one layer or camera to another.
    /// </summary>
    internal static class PosTransform
    {
        /// <summary>
        /// Transforms a position from a layer to another.
        /// </summary>
        /// <param name="position">Position in the original layer.</param>
        /// <param name="original">Original layer.</param>
        /// <param name="target">Layer to transform position to.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 FromLayerToLayer(Vec2 position, Layer original, Layer target)
        {
            return FromCamToCam(position, original.camera, target.camera);
        }

        /// <summary>
        /// Transforms a position from a layer to screen position.
        /// </summary>
        /// <param name="layerPos">Position inside the original layer.</param>
        /// <param name="layer">Original layer.</param>
        /// <returns>Transformed screen position.</returns>
        public static Vec2 ToScreenPos(Vec2 layerPos, Layer layer)
        {
            return ToScreenPos(layerPos, layer.camera);
        }

        /// <summary>
        /// Transforms a screen position to a position inside a layer.
        /// </summary>
        /// <param name="screenPos">Screen position.</param>
        /// <param name="layer">Original layer.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 FromScreenPos(Vec2 screenPos, Layer layer)
        {
            return FromScreenPos(screenPos, layer.camera);
        }

        /// <summary>
        /// Transforms a position from a camera to another.
        /// </summary>
        /// <param name="position">Position in the original camera.</param>
        /// <param name="original">Original camera.</param>
        /// <param name="target">Camera to transform position to.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 FromCamToCam(Vec2 position, Camera original, Camera target)
        {
            Vec2 screenPos = ToScreenPos(position, original);
            return FromScreenPos(screenPos, target);
        }

        /// <summary>
        /// Transforms a position from a camera to screen position.
        /// </summary>
        /// <param name="camPos">Position inside the original layer.</param>
        /// <param name="cam">Original camera.</param>
        /// <returns>Transformed screen position.</returns>
        public static Vec2 ToScreenPos(Vec2 camPos, Camera cam)
        {
            return cam.transform(camPos);
        }

        /// <summary>
        /// Transforms a screen position to a position inside a camera.
        /// </summary>
        /// <param name="screenPos">Screen position.</param>
        /// <param name="cam">Original camera.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 FromScreenPos(Vec2 screenPos, Camera cam)
        {
            return cam.transformInverse(screenPos);
        }

        /// <summary>
        /// Transforms a position from a layer to the Console layer.
        /// </summary>
        /// <param name="position">Position in the original layer.</param>
        /// <param name="layer">Original layer.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 ToConsolePos(Vec2 position, Layer layer)
        {
            return FromLayerToLayer(position, layer, Layer.Console);
        }

        /// <summary>
        /// Transforms a position from a layer to the HUD layer.
        /// </summary>
        /// <param name="position">Position in the original layer.</param>
        /// <param name="layer">Original layer.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 ToHUDPos(Vec2 position, Layer layer)
        {
            return FromLayerToLayer(position, layer, Layer.HUD);
        }

        /// <summary>
        /// Transforms a position from a layer to the Game layer.
        /// </summary>
        /// <param name="position">Position in the original layer.</param>
        /// <param name="layer">Original layer.</param>
        /// <returns>Transformed position.</returns>
        public static Vec2 ToGamePos(Vec2 position, Layer layer)
        {
            return FromLayerToLayer(position, layer, Layer.Game);
        }
    }
}
