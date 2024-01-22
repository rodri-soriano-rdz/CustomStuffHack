using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Harmony;
using DuckGame.CustomShitHack.Images;
using System.IO;
using DuckGame.CustomShitHack.Hacking;
using DuckGame.CustomShitHack.ExtendedHats;

namespace DuckGame.CustomShitHack.Harmony
{
    internal class HarmonyPatches
    {
        /// <summary>
        /// Helps keep track of the levels' original camera for cam manipulation.
        /// </summary>
        [HarmonyPatch(typeof(Level), nameof(Level.DoInitialize))]
        internal static class Level_DoInitialize
        {
            internal static void Postfix(Level __instance)
            {
                HCameraManipulator.OnLevelChange(__instance);
            }
        }

        [HarmonyPatch(typeof(TeamHat), MethodType.Constructor, new Type[] { typeof(float), typeof(float), typeof(Team), typeof(Profile) })]
        internal static class TeamHat_Cctor
        {
            internal static void Postfix(TeamHat __instance)
            {
                SFX.Play("quack");
                if (__instance is TeamHat)
                {
                    var teamHat = __instance as TeamHat;

                    try
                    {
                        if (teamHat.team == null) return;
                        if (!teamHat.isServerForObject) return;
                        if (teamHat.team.name.StartsWith("CSHIMG")) return;
                        if (!teamHat.team.name.StartsWith("DUCK")) return;

                        var img = new BaseImage(CSH_Mod.GetContentPath("Images/singleDuck.png"), Vec2.Zero)
                        {
                            Center = new Vec2(0.5f)
                        }; 
                        ExtendedHatsManager.AddHat(teamHat, img);
                    }
                    catch
                    {
                        SFX.Play("consoleError");
                    }
                }

            }
        }

        [HarmonyPatch(typeof(TeamHat), nameof(TeamHat.Terminate))]
        internal static class TeamHat_Terminate
        {
            internal static void Postfix(TeamHat __instance)
            {
                ExtendedHatsManager.RemoveHat(__instance);
            }
        }

        // FIXME: fix camera glitch.
        //[HarmonyPatch(typeof(FollowCam), nameof(FollowCam.Remove))]
        //internal static class FollowCam_Remove
        //{
        //    internal static void Postfix(FollowCam __instance)
        //    {

        //    }
        //}

        //[HarmonyPatch(typeof(FollowCam), nameof(FollowCam.Add))]
        //internal static class FollowCam_Add
        //{
        //    internal static void Postfix(FollowCam __instance)
        //    {

        //    }
        //}

        /// <summary>
        /// Syncs all added images to new connections.
        /// </summary>
        [HarmonyPatch(typeof(DuckNetwork), nameof(DuckNetwork.Server_AcceptJoinRequest))]
        internal static class DuckNetwork_Server_AcceptJoinRequest
        {
            internal static void Postfix(List<Profile> pJoinedProfiles, bool pLocal)
            {
                // If connection is local, skip sync.
                if (pLocal)
                {
                    return;
                }

                if (pJoinedProfiles == null || pJoinedProfiles.Count == 0)
                {
                    return;
                }

                // Perform sync.
                var connection = pJoinedProfiles.First().connection;

                if (connection != null)
                {
                    TeamSynchronizer.EnqueueConnectionForSync(connection);
                }
            }
        }

        //[HarmonyPatch(typeof(DuckNetwork), nameof(DuckNetwork.OnMessage))]
        //internal static class NMJoinDuckNetSuccess_Deserialize
        //{
        //    internal static void Postfix(List<Profile> pJoinedProfiles, bool pLocal)
        //    {
        //        // If connection is local, skip sync.
        //        if (pLocal)
        //        {
        //            return;
        //        }

        //        if (pJoinedProfiles == null || pJoinedProfiles.Count == 0)
        //        {
        //            return;
        //        }

        //        // Perform sync.
        //        var connection = pJoinedProfiles.First().connection;

        //        if (connection != null)
        //        {
        //            TeamSynchronizer.EnqueueConnectionForSync(connection);
        //        }
        //    }
        //}

        /// <summary>
        /// Clears sync history when a client disconnects.
        /// </summary>
        [HarmonyPatch(typeof(NCNetworkImplementation), nameof(NCNetworkImplementation.DisconnectClient))]
        internal static class NCNetworkImplementation_DisconnectClient
        {
            internal static void Postfix(NetworkConnection connection)
            {
                if (connection == null)
                {
                    return;
                }
                
                TeamSynchronizer.ClearSyncHistoryForConnection(connection);
            }
        }

        /// <summary>
        /// Disables cheat check for commands.
        /// </summary>
        [HarmonyPatch(typeof(DevConsole), "CheckCheats")]
        internal static class DevConsole_CheckCheats
        {
            internal static bool Prefix(ref bool __result)
            {
                __result = false;
                return false;
            }
        }
    }
}
