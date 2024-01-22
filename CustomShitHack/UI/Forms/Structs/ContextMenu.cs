using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DuckGame.CustomShitHack.UI
{
    /// <summary>
    /// Represents a context menu.
    /// </summary>
    internal struct ContextMenu
    {
        /// <summary>
        /// The title of the context menu.
        /// </summary>
        public string Title;

        /// <summary>
        /// Maximum size of the context menu.
        /// </summary>
        public Size MaxSize;

        /// <summary>
        /// Whether the context menu should automatically close after an item is selected.
        /// </summary>
        public bool AutoClose;

        /// <summary>
        /// An array of context menu items contained within the context menu.
        /// </summary>
        /// <remarks></remarks>
        public ContextMenuItem[] Items;

        /// <summary>
        /// Initializes a context menu.
        /// </summary>
        /// <param name="title">The title of the context menu.</param>
        /// <param name="maxSize">The maximum size of the context menu. (Set to default for no maximum size)</param>
        /// <param name="autoClose">Whether the context menu should automatically close (default is true).</param>
        /// <param name="items">An array of <see cref="ContextMenuItem"/> representing the menu items.</param>
        public ContextMenu(string title, Size maxSize = default, bool autoClose = true, params ContextMenuItem[] items)
        {
            Title = title;
            MaxSize = maxSize;
            AutoClose = autoClose;
            Items = items;
        }

        /// <summary>
        /// Initializes a context menu.
        /// </summary>
        /// <param name="title">The title of the context menu.</param>
        /// <param name="items">An array of <see cref="ContextMenuItem"/> representing the menu items.</param>
        public ContextMenu(string title, params ContextMenuItem[] items)
        {
            Title = title;
            MaxSize = default;
            AutoClose = true;
            Items = items;
        }

        /// <summary>
        /// Initializes a context menu.
        /// </summary>
        /// <param name="title">The title of the context menu.</param>
        /// <param name="items">A semicolon-separated string containing the menu items.</param>
        /// <param name="actions">An array of click actions corresponding to each menu item.</param>
        public ContextMenu(string title, string items, params ContextMenuItem.OnClick[] actions)
        {
            var strings = items.Split(';');
            IList<ContextMenuItem> menuItems = new List<ContextMenuItem>();

            var i = 0;

            foreach (var item in strings)
            {
                ContextMenuItem.OnClick action = null;

                if (i < actions.Length)
                {
                    action = actions[i];
                }

                menuItems.Add(new ContextMenuItem(item, action));
                i++;
            }

            Title = title;
            MaxSize = default;
            AutoClose = true;
            Items = menuItems.ToArray();
        }

        /// <summary>
        /// Implicitly converts a string to a <see cref="ContextMenu"/> with the specified title.
        /// </summary>
        /// <param name="title">The title of the context menu.</param>
        /// <returns>A new instance of the <see cref="ContextMenu"/> struct with the specified title.</returns>
        public static implicit operator ContextMenu(string title) => new ContextMenu(title);
    }
}
