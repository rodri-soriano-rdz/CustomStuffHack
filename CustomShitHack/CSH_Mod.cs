using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomStuffHack.Utility;
using DuckGame.CustomStuffHack.Harmony;
using DuckGame.CustomStuffHack.Images;
using System.Reflection;
using DuckGame.CustomStuffHack.ModInput;
using DuckGame.CustomStuffHack.UI;
using DuckGame.CustomStuffHack.Hacking;
using DuckGame.CustomStuffHack.ExtendedHats;

namespace DuckGame.CustomStuffHack
{
    /// <summary>
    /// Main mod class.
    /// </summary>
    public class CSH_Mod : ClientMod
    {
        /// <summary>
        /// Mod's priority. Mods with higher priority load first.
        /// </summary>
        public override Priority priority => Priority.Monitor;

        /// <summary>
        /// Mod config and assembly information.
        /// </summary>
        public static ModConfiguration Configuration;

        /// <summary>
        /// Returns absolute path inside contents folder.
        /// </summary>
        /// <param name="relativePath">Relative path inside contents folder.</param>
        public static string GetContentPath(string relativePath)
        {
            return GetPath<CSH_Mod>(relativePath);
        }

        /// <inheritdoc/>
        protected override void OnPreInitialize()
        {
            Configuration = configuration;

            // Enable assembly loader.
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                return AssemblyLoader.TryLoadAssembly(args.Name);
            };

            base.OnPreInitialize();
        }

        /// <inheritdoc/>
        protected override void OnPostInitialize()
        {
            // Load smallBios font.
            FontLoader.TryLoadFont("smallBiosFontUI", 7, 5, "smallBios");
            FontLoader.TryGetFont("smallBios", out var smallBiosFont);
            smallBiosFont.spriteScale = Vec2.One * 0.8f;

            // Load menuFont.
            FontLoader.TryLoadFont("biosFontUI", 8, 7, "menuFont");
            FontLoader.TryGetFont("smallBios", out var menuFont);
            menuFont.spriteScale = Vec2.One * 0.8f;
            menuFont.singleLine = true;

            HarmonyPatcher.PerformPatches();
            // --- Any initialization that depends on Harmony patches should be done after this point ---

            // Initialize classes.
            MainUpdater.Initialize();
            GameTime.Initialize();
            ModMouse.Initialize();
            ModKeyboard.Initialize();
            ImageManager.Initialize();
            HackManager.Initialize();
            ExtendedHatsManager.Initialize();

            base.OnPostInitialize();
        }

        /// <inheritdoc/>
        protected override void OnStart()
        {
            base.OnStart();
        }
    }
}
