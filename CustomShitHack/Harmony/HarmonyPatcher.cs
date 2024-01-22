using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.Utility;
using Harmony;
using HarmonyLoader;

namespace DuckGame.CustomShitHack.Harmony
{
    /// <summary>
    /// Tool for performing patches using harmony.
    /// </summary>
    internal static class HarmonyPatcher
    {
        public static HarmonyInstance HarmonyInstance => Loader.harmonyInstance;

        /// <summary>
        /// Performs all found patches inside the mod.
        /// </summary>
        public static void PerformPatches()
        {
            Logger.Log("Performing harmony patches...", LogSeverity.Information);

            try
            {
                HarmonyInstance.PatchAll();
                Logger.Log("Performed harmony patches!", LogSeverity.Success);
            }
            catch (Exception e)
            {
                Logger.Log("Could not perform harmony patches!", LogSeverity.Error);
                Logger.Log(e.Message, LogSeverity.Debug);
            }
        }
    }
}
