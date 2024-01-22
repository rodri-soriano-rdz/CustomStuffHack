using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using DuckGame.CustomShitHack.Utility;
using System.Xml.Linq;
using Harmony;

namespace DuckGame.CustomShitHack.Images
{
    internal static class TeamImageFactory
    {
        public const int TILE_SIZE = 32;
        public const string TEAM_PREFIX = "CSHIMG";

        private static readonly IDictionary<string, TeamImage> s_imageIDPool = new Dictionary<string, TeamImage>();

        public static bool AlreadyExists(string imageID)
        {
            return s_imageIDPool.ContainsKey(imageID);
        }

        public static bool TryGet(string imageID,  out TeamImage image)
        {
            return s_imageIDPool.TryGetValue(imageID, out image);
        }

        public static bool GetOrCreateFromPath(string path, out TeamImage image, bool overrideExisting = false)
        {
            // Check if image is already in pool.
            if (AlreadyExists(path))
            {
                if (overrideExisting)
                {
                    // Remove image from pool.
                    s_imageIDPool.Remove(path);
                }
                else
                {
                    // Retrieve image from pool.
                    return s_imageIDPool.TryGetValue(path, out image);
                }
            }

            // Check if path exists.
            if (!File.Exists(path))
            {
                image = new TeamImage();
                return false;
            }

            var bmp = new Bitmap(path);

            // Check if image could be created successfully.
            if (bmp == null)
            {
                image = new TeamImage();
                return false;
            }

            string teamID = Path.GetFileNameWithoutExtension(path);

            // Create image.
            image = FromBitmap(bmp, teamID);
            s_imageIDPool.Add(path, image);
            return true;
        }

        public static TeamImage FromBitmap(Bitmap image, string teamName)
        {
            // Calculate number of tiles.
            var imgSize = new Vec2(image.Width, image.Height);

            int tilesX = (int)Math.Ceiling(imgSize.x / TILE_SIZE);
            int tilesY = (int)Math.Ceiling(imgSize.y / TILE_SIZE);

            var teams = new List<TileTeam>();

            // Split image into sections.
            for (int x = tilesX - 1; x >= 0; x--)
            {
                for (int y = tilesY - 1; y >= 0; y--)
                {
                    // Calculate crop rectangle.
                    Vec2 tileCoords = new Vec2(x, y);
                    Vec2 cropTL = TILE_SIZE * tileCoords;
                    Vec2 remnant = imgSize - cropTL;
                    Vec2 cropSize = Vec2.Zero;
                    cropSize.x = Math.Min(remnant.x, TILE_SIZE);
                    cropSize.y = Math.Min(remnant.y, TILE_SIZE);

                    // Create rectangle.
                    var rect = new System.Drawing.Rectangle
                    {
                        X = (int)cropTL.x,
                        Y = (int)cropTL.y,
                        Size = new Size((int)cropSize.x, (int)cropSize.y)
                    };

                    // Create cropped image section.
                    Bitmap croppedImg = image.Clone(rect, PixelFormat.Format32bppArgb);

                    // Convert image to byte array.
                    ImageConverter converter = new ImageConverter();
                    byte[] imgData = (byte[])converter.ConvertTo(croppedImg, typeof(byte[]));

                    // Create tileTeam.
                    string name = $"{TEAM_PREFIX}|{teamName}|{x},{y}";

                    int index = 0;

                    // Make sure team name is unique.
                    while (Teams.all.FirstOrDefault(t=>t.name == name) != null)
                    {
                        index++;
                        name = $"{TEAM_PREFIX}|{teamName}_{index}|{x},{y}";
                    }

                    var team = Team.DeserializeFromPNG(imgData, name, null);
                    var tileTeam = new TileTeam(team, tileCoords);
                    teams.Add(tileTeam);

                    // Add team to all teams.
                    Teams.AddExtraTeam(team);

                    // Free resources.
                    croppedImg.Dispose();
                }
            }

            // Create team image and return it.
            return new TeamImage(teams.ToArray(), new Vec2(tilesX, tilesY), imgSize);
        }
    }
}
