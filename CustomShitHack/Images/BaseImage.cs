using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Dynamic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using DuckGame.CustomShitHack.Utility;
using Harmony;

namespace DuckGame.CustomShitHack.Images
{
    internal class BaseImage
    {
        public const float OFFSCREEN_PADDING = -100;

        public readonly TeamImage TeamImage;
        public Vec2 Center = Vec2.Zero;
        public Vec2 Position;
        public float Angle = 0f;
        public float Depth = 0f;
        public Level Level;
        public bool PersistThroughLevels = false;
        public bool Regenerate = true;
        public bool Visible = true;
        public bool FlipH = false;
        // TODO: Depth synchronization.

        protected List<TileHat> m_tileHats;
        protected List<Vec2> m_offScreenHatsCoords = new List<Vec2>();
        protected ScoreRock m_depthRock;

        public Vec2 CenterPosition => Position - TeamImage.ImageSize * Center;

        public BaseImage(
            TeamImage teamImage,
            Vec2 position,
            bool regenerate = true,
            bool persistentThroughLevels = false)
        {
            TeamImage = teamImage;
            Position = position;
            Regenerate = regenerate;
            PersistThroughLevels = persistentThroughLevels;
        }

        public BaseImage(
            string path,
            Vec2 position,
            bool regenerate = true,
            bool persistentThroughLevels = false)
        {
            TeamImageFactory.GetOrCreateFromPath(path, out TeamImage);
            Position = position;
            Regenerate = regenerate;
            PersistThroughLevels = persistentThroughLevels;
        }

        public BaseImage(
            Bitmap bmp,
            Vec2 position,
            bool regenerate = true,
            bool persistentThroughLevels = false)
        {
            TeamImage = TeamImageFactory.FromBitmap(bmp, "image");
            Position = position;
            Regenerate = regenerate;
            PersistThroughLevels = persistentThroughLevels;
        }

        public virtual void Initialize()
        {
            m_tileHats = CreateTileHats();
            Level = Level.current;

            m_depthRock = new ScoreRock(0f, 0f, null)
            {
                depth = Depth
            };

            // Add all hats.
            foreach (TileHat hat in m_tileHats)
            {
                Level.Add(hat.TeamHat);
            }

            AlignHats();

            ImageManager.StartUpdatingImage(this);
        }

        public virtual void Terminate()
        {
            // Remove all hats.
            foreach(TileHat tileHat in m_tileHats)
            {
                Level.Remove(tileHat.TeamHat);
            }

            // Reset properties.
            Level = null;
            m_tileHats.Clear();
            m_offScreenHatsCoords.Clear();

            ImageManager.StopUpdatingImage(this);
        }

        public virtual void Update()
        {
            AlignHats();

            if (Level.current != Level)
            {
                // Terminate if level changed and image is not persistent.
                if (!PersistThroughLevels)
                {
                    Terminate();
                }
                // Regenerate image if level changed.
                else
                {
                    for (int i = m_tileHats.Count - 1; i >= 0; i--)
                    {
                        RegenHat(i);
                    }

                    m_offScreenHatsCoords.Clear();
                    Level = Level.current;
                }
            }

            // Terminate if all hats were destroyed and image does not regen.
            if (m_tileHats.Count == 0 && !Regenerate)
            {
                Terminate();
            }
        }

        public virtual void UpdateOptimized()
        {
            DoHatsRegen();
        }

        public virtual void Sync(NetworkConnection connection = null)
        {
            foreach (var tileTeam in TeamImage.Teams)
            {
                TeamSynchronizer.EnqueueTeamForSynchronization(tileTeam.Team, connection);
            }
        }

        public virtual Vec2 GetPositionAt(Vec2 posNormalized)
        {
            Vec2 pos = Position;
            pos -= (Position - CenterPosition);
            pos += TeamImage.ImageSize * posNormalized;

            var angle = Angle;
            if (FlipH)
            {
                angle *= -1;
                angle -= (float)Math.PI / 2f;
            }

            pos = pos.Rotate(angle, Position);
            return pos;
        }

        protected virtual void AlignHats()
        {
            for (int i = m_tileHats.Count - 1; i >= 0; i--)
            {
                TileHat tileHat = m_tileHats[i];
                TeamHat teamHat = tileHat.TeamHat;

                if (!teamHat.isServerForObject)
                {
                    Thing.PowerfulRuleBreakingFondle(teamHat, DuckNetwork.localConnection);
                }

                // Skip if hat has no graphic.
                if (teamHat.graphic == null)
                {
                    continue;
                }

                var pos = CenterPosition;
                var angle = Angle;

                Vec2 coords = tileHat.Coordinates;
                Vec2 center = teamHat.graphic.Size() * 0.5f;
                center = center.Floor();

                // Flip image if needed.
                if (FlipH)
                {
                    coords.x = TeamImage.TotalTiles.x - tileHat.Coordinates.x;
                    pos += center * new Vec2(-1, 1);
                    if (TeamImage.ImageSize.x % TeamImageFactory.TILE_SIZE != 0)
                    {
                        coords.x -= 1;
                    }
                }
                else
                {
                    pos += center;
                }

                // Check if image is outside camera boundaries.
                pos += coords * TeamImageFactory.TILE_SIZE;
                pos = pos.Rotate(angle, Position);

                Vec2 camTL = Layer.Game.camera.position;
                Vec2 camBR = camTL + Layer.Game.camera.size;

                camTL += new Vec2(OFFSCREEN_PADDING);
                camBR -= new Vec2(OFFSCREEN_PADDING);

                Rectangle camRect = new Rectangle(camTL, camBR);

                // Remove off-screen hats.
                if (!camRect.Contains(pos))
                {
                    if (!m_offScreenHatsCoords.Contains(tileHat.Coordinates))
                    {
                        m_offScreenHatsCoords.Add(tileHat.Coordinates);
                        Level.Remove(teamHat);
                    }
                }
                // Add previously off-screen hats back.
                else 
                {
                    if (m_offScreenHatsCoords.Contains(tileHat.Coordinates))
                    {
                        m_offScreenHatsCoords.Remove(tileHat.Coordinates);
                        RegenHat(i);
                    }
                }

                teamHat.position = pos;
                teamHat.angle = angle;
                teamHat.flipHorizontal = FlipH;
            }
        }

        protected virtual void RegenHat(int index)
        {
            if (m_tileHats.Count <= index || index < 0) return;

            TileHat tileHat = m_tileHats[index];
            TeamHat hat = tileHat.TeamHat;

            // Create new hat.
            var newHat = new TeamHat(0f, 0f, hat.team)
            {
                gravMultiplier = 0,
                canPickUp = false
            };
            var newTileHat = new TileHat(newHat, tileHat.Coordinates);

            // Remove old hat.
            Level.Remove(hat);
            m_tileHats.Remove(tileHat);

            // Add new hat.
            Level.Add(newHat);
            m_tileHats.Add(newTileHat);
        }

        protected virtual void DoHatsRegen()
        {
            if (!Regenerate) return;

            for (int i = m_tileHats.Count - 1; i >= 0; i--)
            {
                TileHat tileHat = m_tileHats[i];
                TeamHat hat = tileHat.TeamHat;

                // Do not regen if hat is off-screen.
                if (m_offScreenHatsCoords.Contains(tileHat.Coordinates))
                {
                    continue;
                }

                // Skip regeneration if hat is under level's bottom height limit.
                // +500f is added to lowest point, since that's the point where PhysicsObject
                // are removed from the level.
                if (hat.position.y > Level.current.lowestPoint + 500f)
                {
                    continue;
                }

                // Check if hat needs to be regenerated.
                if (hat.alpha >= 1 && !hat.destroyed && !hat.removeFromLevel)
                {
                    continue;
                }

                RegenHat(i);
            }
        }

        protected virtual List<TileHat> CreateTileHats()
        {
            var hats = new List<TileHat>();

            foreach (TileTeam tileTeam in TeamImage.Teams)
            {
                var hat = new TeamHat(0f, 0f, tileTeam.Team)
                {
                    gravMultiplier = 0,
                    canPickUp = false
                };
                var tileHat = new TileHat(hat, tileTeam.Coordinates);

                hats.Add(tileHat);
            }

            return hats;
        }
    }
}
