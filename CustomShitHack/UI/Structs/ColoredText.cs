using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DuckGame.CustomStuffHack.UI
{
    internal struct ColoredText
    {
        public string Text;
        public Color Color;

        public ColoredText(string text, Color color = default)
        {
            Text = text;
            Color = color;
        }

        public static implicit operator string(ColoredText coloredText) => coloredText.Text;
        public static implicit operator Color(ColoredText coloredText) => coloredText.Color;
        public static implicit operator ColoredText(string text) => new ColoredText(text);

        public override string ToString()
        {
            return Text;
        }
    }
}
