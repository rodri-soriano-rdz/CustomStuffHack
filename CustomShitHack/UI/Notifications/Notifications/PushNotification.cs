using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.Utility;

namespace DuckGame.CustomShitHack.UI
{
    internal class PushNotification : Notification
    {
        public delegate void PressAction();

        public Color ActionTextColor = ColorTable.TEXT;
        protected PressAction m_action;

        protected string m_actionText = "";
        protected string m_input = "SHOOT";
        protected string m_pressSound = "highClick";

        public PushNotification(string title = "", string subtitle = "", uint duration = 0, string actionText = "", PressAction action = null) : base(title, subtitle, duration)
        {
            m_actionText = actionText;
            m_action = action;
        }

        public virtual void OnKeyPress()
        {
            m_action?.Invoke();
            PlayPressSound();
            MultSlideSpeed(2f);
            TryTerminate();
        }

        public virtual void PlayPressSound()
        {
            SFX.Play(m_pressSound, 1, 0, 0, false);
        }

        public override void Update()
        {
            if (Input.Pressed(m_input) && LifeNorm < 1f) OnKeyPress();

            base.Update();
        }

        public override void Draw()
        {
            base.Draw();

            float scale = 0.5f;

            // Load fonts.
            var font = FontLoader.GetFontOrDefault("bios");
            var actionFont = FontLoader.GetFontOrDefault("smallBios");

            // Title font initialization.
            Vec2 prevScaleT = font.scale;
            Vec2 prevSprScaleT = font.spriteScale;
            bool prevSingleLineT = font.singleLine;

            // Subtitle font initialization.
            Vec2 prevScaleST = actionFont.scale;
            Vec2 prevSprScaleST = actionFont.spriteScale;
            bool prevSingleLineST = actionFont.singleLine;

            // Title font.
            font.scale = new Vec2(scale);
            font.spriteScale = font.scale;
            font.singleLine = false;

            // Subtitle font.
            actionFont.scale = new Vec2(scale);
            actionFont.spriteScale = Vec2.One * 0.5f;
            actionFont.singleLine = true;

            // Calculate key text position.
            string text = m_actionText;
            string mojiText = $"@{m_input}@";
            float mojiWidth = font.GetWidth("  ");
            float texWidth = font.GetWidth(text);
            float totalWidth = texWidth + mojiWidth;

            float margin = 4f * scale;
            Vec2 texPos = Center;
            texPos.x -= totalWidth / 2f;
            texPos.y += Size.y / 2f;
            texPos.y += margin;
            
            // Notification offset.
            float posOffX = Math.Max(0, totalWidth - Size.x);
            posOffX /= 2f;

            m_positionOffset.x = posOffX;

            actionFont.Draw(mojiText, texPos, ColorTable.TEXT, DepthTable.NOTIFICATION_TEXT, Profiles.DefaultPlayer1.inputProfile);

            texPos.x += mojiWidth;

            actionFont.DrawOutline(text, texPos, ActionTextColor, ColorTable.BACKGROUND, DepthTable.NOTIFICATION_TEXT);

            // Font termination.
            font.scale = prevScaleT;
            font.spriteScale = prevSprScaleT;
            font.singleLine = prevSingleLineT;

            // Subtitle font termination.
            actionFont.scale = prevScaleST;
            actionFont.spriteScale = prevSprScaleST;
            actionFont.singleLine = prevSingleLineST;
        }
    }
}
