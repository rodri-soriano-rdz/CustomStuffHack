using System.Collections.Generic;
using System;
using System.Windows;
using DuckGame.CustomStuffHack.Images;

namespace DuckGame.CustomStuffHack.Utility
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Splits a string into smaller strings given a maximum length.
        /// </summary>
        public static IEnumerable<string> SplitByLength(this string str, int maxLength)
        {
            for (int index = 0; index < str.Length; index += maxLength)
            {
                yield return str.Substring(index, Math.Min(maxLength, str.Length - index));
            }
        }

        /// <summary>
        /// Returns the size of a sprite.
        /// </summary>
        public static Vec2 Size(this Sprite spr, bool scaled = false)
        {
            var ret = new Vec2(spr.width, spr.height);

            if (scaled)
            {
                ret *= spr.scale;
            }

            return ret;
        }

        /// <summary>
        /// Returns vector with it's components floored.
        /// </summary>
        public static Vec2 Floor(this Vec2 vec)
        {
            Vec2 ret = vec;
            ret.x = (float)Math.Floor(vec.x);
            ret.y = (float)Math.Floor(vec.y);
            return ret;
        }

        /// <summary>
        /// Returns vector with it's components ceiled.
        /// </summary>
        public static Vec2 Ceiling(this Vec2 vec)
        {
            Vec2 ret = vec;
            ret.x = (float)Math.Ceiling(vec.x);
            ret.y = (float)Math.Ceiling(vec.y);
            return ret;
        }

        /// <summary>
        /// Returns vector with it's components' absolute value.
        /// </summary>
        public static Vec2 Abs(this Vec2 vec)
        {
            Vec2 ret = vec;
            ret.x = (float)Math.Abs(vec.x);
            ret.y = (float)Math.Abs(vec.y);
            return ret;
        }
        
        /// <summary>
        /// Checks if Team corresponds to a team image.
        /// </summary>
        /// <param name="team">Team to check.</param>
        public static bool IsImageTeam(this Team team)
        {
            return team.name.StartsWith(TeamImageFactory.TEAM_PREFIX);
        }

        /// <summary>
        /// Checks if TeamHat corresponds to a team image.
        /// </summary>
        /// <param name="teamHat">TeamHat to check.</param>
        public static bool IsImageTeamHat(this TeamHat teamHat)
        {
            if (teamHat.team == null) return false;
            return teamHat.team.IsImageTeam();
        }
    }
}