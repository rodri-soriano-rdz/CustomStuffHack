using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.Utility;
using System.Windows.Forms;
using DuckGame.CustomShitHack.ModInput;

namespace DuckGame.CustomShitHack
{
    internal class MainUpdater : IEngineUpdatable
    {
        public static MainUpdater Instance { get; private set; }

        public static EventHandler OnPreUpdate;
        public static EventHandler OnUpdate;
        public static EventHandler OnPostUpdate;
        public static EventHandler<OnDrawEventArgs> OnDraw;

        public static void Initialize()
        {
            Logger.Log("Initializing updater singleton...", LogSeverity.Information);
            // Check if the instance was already initialized.
            if (Instance == null)
            {
                Instance = new MainUpdater();
                MonoMain.RegisterEngineUpdatable(Instance);

                Logger.Log("Initialized updater singleton!", LogSeverity.Success);
            }
            else
            {
                Logger.Log("Updater singleton was already initialized!", LogSeverity.Warning);
            }
        }

        // Private constructor to prevent external instantiation.
        private MainUpdater() { }

        public void OnDrawLayer(Layer layer)
        {
            OnDraw?.Invoke(new object(), new OnDrawEventArgs(layer));
        }

        public void PreUpdate()
        {
            OnPreUpdate?.Invoke(new object(), null);
        }

        public void Update()
        {
            OnUpdate?.Invoke(new object(), null);
        }

        /// <summary>
        /// Runs even when gameplay should be paused.
        /// </summary>
        /// <remarks>e.g game paused, level changing, etc.</remarks>
        public void PostUpdate()
        {
            OnPostUpdate?.Invoke(new object(), null);
        }
    }
}
