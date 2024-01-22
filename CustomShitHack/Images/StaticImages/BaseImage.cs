using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomShitHack.ImageSystem
{
    /// <summary>
    /// Static synchronized image.
    /// </summary>
    /// <remarks>
    /// Has the simplest behaviour. No regeneration, no updating, no moving.
    /// </remarks>
    internal class BaseImage
    {
        public const int TILE_SIZE = 32;

        protected ImageSection m_firstSection;

        /// <summary>
        /// First image section of the singly linked list.
        /// </summary>
        public ImageSection FirstSection => m_firstSection;

        /// <summary>
        /// Static synchronized image.
        /// </summary>
        /// <remarks>
        /// Has the simplest behaviour. No regeneration, no updating, no moving.
        /// </remarks>
        public BaseImage(ImageSection firstSection)
        {
            m_firstSection = firstSection;
        }

        /// <summary>
        /// Creates an instance of the image at the given position.
        /// </summary>
        public virtual List<TeamHat> CreateInstance(Vec2 pos)
        {
            var hats = new List<TeamHat>();
            ImageSection first = m_firstSection;

            // Iterate through singly linked list.
            while (first.NextSection != null)
            {
                // Create hat.
                var hat = new TeamHat(0f, 0f, first.Team)
                {
                    canPickUp = false,
                    gravMultiplier = 0f
                };

                Vec2 posOffset = first.Team.hat.Size() * 0.5f;
                posOffset += first.RelativePosition;
                Vec2 hatPos = pos + posOffset;
                hatPos += hat.sprite.Size() * 0.5f;

                hat.position = hatPos;

                hats.Add(hat);
                Level.Add(hat);

                first = first.NextSection;
            }

            return hats;
        }
    }
}
