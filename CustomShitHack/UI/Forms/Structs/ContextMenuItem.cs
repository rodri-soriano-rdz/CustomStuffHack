using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;
using DuckGame.CustomStuffHack.Utility;

/// <summary>
/// Represents an item within a context menu.
/// </summary>
internal struct ContextMenuItem
{
    /// <summary>
    /// The text label displayed for the context menu item.
    /// </summary>
    public string Text;

    /// <summary>
    /// The action to be executed when the context menu item is clicked.
    /// </summary>
    public OnClick Action;

    /// <summary>
    /// Whether the context menu item is displayed with a smaller size.
    /// </summary>
    public bool Small;

    /// <summary>
    /// An array of child context menu items, forming a drop down menu if present.
    /// </summary>
    public ContextMenuItem[] Children;

    /// <summary>
    /// Action to be executed when the context menu item is clicked.
    /// </summary>
    public delegate void OnClick();

    /// <summary>
    /// Initializes a <see cref="ContextMenuItem"/>.
    /// </summary>
    /// <param name="text">The text label displayed for the context menu item.</param>
    /// <param name="action">The action to be executed when the context menu item is clicked (default is null).</param>
    /// <param name="small">Indicates whether the context menu item is displayed with a smaller size (default is false).</param>
    /// <param name="children">An array of <see cref="ContextMenuItem"/> objects representing child menu items, forming a drop down menu if present.</param>
    public ContextMenuItem(string text, OnClick action = null, bool small = false, params ContextMenuItem[] children)
    {
        Text = text;
        Action = action;
        Small = small;
        Children = children;
    }

    /// <summary>
    /// Implicitly converts a string to a <see cref="ContextMenuItem"/> with the specified text label.
    /// </summary>
    /// <param name="text">The text label displayed for the context menu item.</param>
    /// <returns>A new instance of the <see cref="ContextMenuItem"/> struct with the specified text label.</returns>
    public static implicit operator ContextMenuItem(string text) => new ContextMenuItem(text);
}
