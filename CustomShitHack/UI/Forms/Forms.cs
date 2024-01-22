using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DuckGame.CustomShitHack.Utility;
using Label = System.Windows.Forms.Label;
using TextBox = System.Windows.Forms.TextBox;
using Button = System.Windows.Forms.Button;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace DuckGame.CustomShitHack.UI
{
    /// <summary>
    /// Provides utility methods for managing forms and user interface elements.
    /// </summary>
    internal static class Forms
    {
        /// <summary>
        /// Represents an action to be executed when file selection is complete.
        /// </summary>
        public delegate void OpenFileDoneAction(string[] filePaths);

        // Private fields and constants.
        private static readonly FieldInfo WINDOW_FIELD = typeof(Resolution).GetField("_window", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly IList<ContextMenuStrip> s_openedContextMenus = new List<ContextMenuStrip>();

        private static readonly Font FONT = new Font("Consolas", 12f, FontStyle.Regular);

        /// <summary>
        /// Gets the game's main window form.
        /// </summary>
        public static Form MainWindow => WINDOW_FIELD.GetValue(null) as Form;

        // Constructor for setting up event handlers.
        static Forms()
        {
            MainWindow.LostFocus += (sender, args) =>
            {
                CloseMenus();
            };

            MainWindow.MouseClick += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    CloseMenus();
                }
            };
        }

        /// <summary>
        /// Opens a file selection dialog for user file selection.
        /// </summary>
        /// <param name="title">The title of the file selection dialog.</param>
        /// <param name="filter">The file filter for allowed file types.</param>
        /// <param name="multiselect">True if multiple files can be selected; otherwise, false.</param>
        /// <param name="action">The action to be executed when file selection is complete.</param>
        /// <param name="initialDir">The initial directory to be displayed in the dialog.</param>
        /// <param name="fileNames">An array of default file names to be displayed in the dialog.</param>
        public static async void OpenFile(string title, string filter, bool multiselect, OpenFileDoneAction action, string initialDir = "", string[] fileNames = default)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filter,
                Multiselect = multiselect,
                InitialDirectory = initialDir
            };

            // Show dialog in a separate thread.
            await StartSTATask(() =>
            {
                ChangeWindowMode();
                dialog.ShowDialog();
            });

            MainWindow.Focus();

            action.Invoke(dialog.FileNames);
        }

        /// <summary>
        /// Opens a context menu at mouse position.
        /// </summary>
        /// <param name="menu">The context menu to be displayed.</param>
        public static void OpenContextMenu(ContextMenu menu)
        {
            // Create menu.
            var menuControl = CreateMenu(menu);

            // Events.
            menuControl.Opened += (invoker, args) =>
            {
                s_openedContextMenus.Add(menuControl);
            };
            menuControl.Closed += (invoker, args) =>
            {
                s_openedContextMenus.Remove(menuControl);
            };

            // Show context menu.
            menuControl.Show(MainWindow, (int)Mouse.mousePos.x, (int)Mouse.mousePos.y);
        }

        public static void ShowTooltip(string tooltip, int x, int y)
        {
            var tooltipControl = new System.Windows.Forms.ToolTip
            {
                ToolTipTitle = tooltip,
                UseAnimation = true,
                UseFading = true,
                IsBalloon = true,
                ReshowDelay = 0,
                InitialDelay = 0,
                AutomaticDelay = 0,
                AutoPopDelay = 10
            };
            tooltipControl.Show(tooltip, MainWindow, x, y, 1000);
        }

        /// <summary>
        /// Starts a task in STA (Single-Threaded Apartment) mode, suitable for forms.
        /// </summary>
        private static Task StartSTATask(Action func)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new Thread(() =>
            {
                try
                {
                    func();
                    tcs.SetResult(null);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }

        /// <summary>
        /// Enables windowed fullscreen if fullscreen is enabled.
        /// </summary>
        private static void ChangeWindowMode()
        {
            if (Resolution.current.mode == ScreenMode.Fullscreen)
            {
                Resolution.Set(Options.LocalData.windowedFullscreenResolution);
                Options.Data.windowedFullscreen = true;
                Options.Save();
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Closes all open context menus.
        /// </summary>
        private static void CloseMenus()
        {
            var menus = s_openedContextMenus.ToArray();
            foreach (var menu in menus)
            {
                menu.Close();
            }
        }

        /// <summary>
        /// Creates a ToolStripMenuItem from a ContextMenuItem.
        /// </summary>
        private static ToolStripMenuItem CreateMenuItem(ContextMenuItem menuItem)
        {
            // Create item.
            var item = new ToolStripMenuItem(menuItem.Text)
            {
                Font = FONT
            };

            if (menuItem.Small)
            {
                item.Font = new Font(FONT.FontFamily, 8f);
                item.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                item.AutoSize = false;
                item.TextAlign = ContentAlignment.MiddleLeft;
                item.Height = 22;
            }

            // Events.
            if (menuItem.Children?.Length > 0)
            {
                item.DropDownOpened += (invoker, args) =>
                {
                    SFX.Play("openClick");
                };
                item.DropDownClosed += (invoker, args) =>
                {
                    SFX.Play("lowClick");
                };
            }
            else
            {
                item.Click += (sender, e) =>
                {
                    try
                    {
                        menuItem.Action?.Invoke();

                        SFX.Play("menuBlip01");
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex.Message, LogSeverity.Error);
                        SFX.Play("consoleError");
                    }
                };
            }

            // Create children items.
            if (menuItem.Children != null)
            {
                foreach (var child in menuItem.Children)
                {
                    var childItem = CreateMenuItem(child);
                    item.DropDownItems.Add(childItem);
                }
            }

            return item;
        }

        /// <summary>
        /// Creates a ContextMenuStrip from a ContextMenu.
        /// </summary>
        private static ContextMenuStrip CreateMenu(ContextMenu menu)
        {
            // Create context menu.
            ContextMenuStrip menuStrip = new ContextMenuStrip
            {
                AutoClose = menu.AutoClose,
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                Opacity = 0.85f,
                Font = FONT,
                DropShadowEnabled = false
            };

            if (menu.MaxSize != default) menuStrip.MaximumSize = menu.MaxSize;

            // Events.
            menuStrip.MouseWheel += (sender, args) =>
            {
                SendKeys.SendWait(args.Delta >= 0 ? "{UP}" : "{DOWN}");
            };
            menuStrip.Opened += (sender, args) =>
            {
                SFX.Play("openClick");
            };
            menuStrip.Closed += (sender, args) =>
            {
                SFX.Play("lowClick");
            };

            // Create title.
            var titleItem = new ToolStripLabel(menu.Title.ToUpperInvariant())
            {
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(menuStrip.Font.FontFamily, 12f, FontStyle.Bold | FontStyle.Italic)
            };
            var separator = new ToolStripSeparator();
            menuStrip.Items.Add(titleItem);
            menuStrip.Items.Add(separator);

            // Add items.
            foreach (var item in menu.Items)
            {
                var menuItem = CreateMenuItem(item);
                menuStrip.Items.Add(menuItem);
            }

            return menuStrip;
        }
    }
}
