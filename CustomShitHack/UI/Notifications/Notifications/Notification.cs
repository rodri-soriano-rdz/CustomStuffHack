using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomShitHack.Utility;

namespace DuckGame.CustomShitHack.UI
{
    internal class Notification : ICloneable
    {
        protected static readonly Vec2 C_GLOBAL_POS = new Vec2(Layer.HUD.width, Layer.HUD.height / 3f);

        public Vec2 MinSize = new Vec2(-1f);

        public Vec2 MaxSize = new Vec2(-1f);

        public Sprite DecorationIcon = null;

        public string Sound = "ching";

        /// <summary>
        /// Notification's icon position offset on the HUD layer.
        /// </summary>
        protected Vec2 IconPosOffset = Vec2.Zero;

        /// <summary>
        /// Size the notification will take.
        /// </summary>
        /// <remarks>Set to -1 for automatic sizing.</remarks>
        protected Vec2 m_size = new Vec2(-1f);

        /// <summary>
        /// Notification's position offset.
        /// </summary>
        protected Vec2 m_positionOffset = new Vec2(0f);

        /// <summary>
        /// Title color.
        /// </summary>
        protected Color m_titleColor = ColorTable.TEXT;

        /// <summary>
        /// Subtitle color.
        /// </summary>
        protected Color m_subtitleColor = ColorTable.SUBTEXT;

        /// <summary>
        /// Background color.
        /// </summary>
        protected Color m_bgColor = ColorTable.BACKGROUND;

        /// <summary>
        /// Time, in frames, the notification will be displayed for.
        /// </summary>
        protected uint m_duration = 0;

        /// <summary>
        /// Time, in frames, the notification has been displayed for.
        /// </summary>
        protected uint m_life = 0;

        /// <summary>
        /// Time, in frames, the animation will last for.
        /// </summary>
        protected uint m_animDuration = 30;

        /// <summary>
        /// Top-left position of the notification.
        /// </summary>
        /// <remarks>To change notification's position, use PositionOffset instead.</remarks>
        private Vec2 m_basePosition = C_GLOBAL_POS;

        /// <summary>
        /// Whether the notification is currently being displayed.
        /// </summary>
        private bool m_active = false;

        /// <summary>
        /// Title of the notification.
        /// </summary>
        private string m_title = "";

        /// <summary>
        /// Subtitle of the notification.
        /// </summary>
        private string m_subtitle = "";

        /// <summary>
        /// Total title lines. Automatically calculated.
        /// </summary>
        private uint m_titleLines = 1;

        /// <summary>
        /// Total subtitle lines. Automatically calculated.
        /// </summary>
        private uint m_subtitleLines = 0;

        /// <summary>
        /// Current animation frame.
        /// </summary>
        private uint m_animFrame = 0;

        /// <summary>
        /// Title of the notification.
        /// </summary>
        public string Title
        {
            get => m_title;

            set
            {
                m_title = value;
                m_titleLines = (uint)value.Split(new string[] { "\n" }, StringSplitOptions.None).Length;
            }
        }

        /// <summary>
        /// Subtitle of the notification.
        /// </summary>
        public string Subtitle
        {
            get => m_subtitle;
            set
            {
                m_subtitle = value;

                if (value == "")
                {
                    m_subtitleLines = 0;
                }
                else m_subtitleLines = (uint)value.Split(new string[] { "\n" }, StringSplitOptions.None).Length;
            }
        }

        /// <summary>
        /// Size the notification will take.
        /// </summary>
        /// <remarks>Set to -1 for automatic sizing.</remarks>
        public Vec2 Size
        {
            get => m_size;
            set
            {
                MaxSize = value;
                MinSize = value;
            }
        }

        /// <summary>
        /// Notification's position offset.
        /// </summary>
        public Vec2 PositionOffset
        {
            get => m_positionOffset;
            set => m_positionOffset = value;
        }

        /// <summary>
        /// Center position of the notification.
        /// </summary>
        public Vec2 Center
        {
            get
            {
                Vec2 pos = m_basePosition;
                pos += m_size / 2f;

                return pos;
            }
        }

        /// <summary>
        /// Top-left position of the notification.
        /// </summary>
        public Vec2 TL => m_basePosition;

        /// <summary>
        /// Top-right position of the notification.
        /// </summary>
        public Vec2 TR
        {
            get
            {
                Vec2 pos = TL;
                pos.x += m_size.x;

                return pos;
            }
        }

        /// <summary>
        /// Bottom-left position of the notification.
        /// </summary>
        public Vec2 BL
        {
            get
            {
                Vec2 pos = TL;
                pos.y += m_size.y;

                return pos;
            }
        }

        /// <summary>
        /// Bottom-right position of the notification.
        /// </summary>
        public Vec2 BR
        {
            get
            {
                Vec2 pos = TR;
                pos.y += m_size.y;

                return pos;
            }
        }

        /// <summary>
        /// Number from 0 to 1 that represents notification's animation.
        /// </summary>
        public float AnimationNorm => (float)m_animFrame / m_animDuration;

        /// <summary>
        /// Number from 0 to 1 that represents notification's total life.
        /// </summary>
        public float LifeNorm => (float)m_life / m_duration;

        /// <summary>
        /// Whether this notification will replace other notifications of same type in the queue.
        /// </summary>
        public bool Active => m_active;

        /// <summary>
        /// Mojis Plus custom notification.
        /// </summary>
        public Notification(string title = "", string subtitle = "", uint duration = 0)
        {
            Title = title;
            Subtitle = subtitle;
            m_duration = duration;
        }

        /// <summary>
        /// Initializes notification.
        /// </summary>
        /// <returns>True if notification was initialized; false if it was already active.</returns>
        /// <param name="playSound">Whether the sound effect will be played.</param>
        /// <param name="slide">Whether the slide-in animation will be played.</param>
        public virtual bool TryInitialize(bool playSound = true, bool slide = true)
        {
            if (m_active) return false;

            m_life = 0;
            m_animFrame = slide ? 0 : m_animDuration;
            m_active = true;

            if (playSound) SFX.Play(Sound, 1f, 0f, 0f, false);

            return true;
        }

        /// <summary>
        /// Terminates notification.
        /// </summary>
        /// <returns>True if notification was terminated; false it it was already inactive.</returns>
        /// <param name="slide">Whether the slide-out animation will be played.</param>
        public virtual bool TryTerminate(bool slide = true)
        {
            if (!m_active) return false;
            if (slide && m_animFrame < m_animDuration && m_life > 0) return false;

            m_life = m_duration;
            m_animFrame = slide ? m_animDuration : 0;
            m_active = slide;

            return true;
        }

        /// <summary>
        /// Restarts notification.
        /// </summary>
        public virtual void Restart(bool playSound = false, bool slide = true)
        {
            m_life = 0;
            m_animFrame = slide ? 0 : m_animDuration;
            m_active = true;

            if (playSound) SFX.Play(Sound, 1f, 0f, 0f, false);
        }

        /// <summary>
        /// Sets notification's slide in-out animation speed.
        /// </summary>
        /// <param name="speedMul">Speed multiplier.</param>
        public virtual void MultSlideSpeed(float speedMul)
        {
            float speed = 1f / speedMul;
            m_animFrame = (uint)Math.Round(speed * m_animFrame);
            m_animDuration = (uint)Math.Round(speed * m_animDuration);
        }

        /// <summary>
        /// Sets notification's slide in-out animation speed.
        /// </summary>
        /// <param name="speed">Animation speed.</param>
        public virtual void SetSlideSpeed(uint speed)
        {
            float scale = (float)speed / m_animDuration;

            m_animFrame = (uint)Math.Round(m_animFrame * scale);
            m_animDuration = speed;
        }

        /// <summary>
        /// Runs every frame. Updates notification.
        /// </summary>
        public virtual void Update()
        {
            // Fade in animation.
            if (m_animFrame <= m_animDuration && m_life <= 0) m_animFrame++;

            // Increase life if slide in animation is over.
            if (m_animFrame >= m_animDuration) m_life++;

            // Fade out animation.
            if (m_life >= m_duration)
            {
                m_animFrame--;

                if (m_animFrame <= 0)
                {
                    TryTerminate(false);
                }
            }
        }

        /// <summary>
        /// Runs every frame. Draws notification.
        /// </summary>
        public virtual void Draw()
        {
            float scale = 0.5f;

            // Load fonts.
            var titleFont = FontLoader.GetFontOrDefault("bios");
            var subtitleFont = FontLoader.GetFontOrDefault("smallBios");

            // Title font initialization.
            Vec2 prevScaleT = titleFont.scale;
            Vec2 prevSprScaleT = titleFont.spriteScale;
            bool prevSingleLineT = titleFont.singleLine;

            // Subtitle font initialization.
            Vec2 prevScaleST = subtitleFont.scale;
            Vec2 prevSprScaleST = subtitleFont.spriteScale;
            bool prevSingleLineST = subtitleFont.singleLine;

            // Title font.
            titleFont.scale = new Vec2(scale);
            titleFont.spriteScale = titleFont.scale;
            titleFont.singleLine = false;

            float tFontHeight = titleFont.height;

            // Title calculations.
            float titleWidth = titleFont.GetWidth(m_title.ToString());
            float titleHeight = (m_title == "") ? 0 : tFontHeight * m_titleLines;
            Vec2 titlePos = C_GLOBAL_POS;
            titlePos.y += m_positionOffset.y;

            // Add margin.
            Vec2 margin = new Vec2(5f, 2f) * scale;

            Vec2 marginWide = new Vec2(1.5f * scale, 0f);
            Vec2 marginHigh = new Vec2(0f, 1.5f * scale);

            float totalMarginX = margin.x * 2f;
            float totalMarginY = margin.y * 2f;

            titlePos.x += margin.x;
            titlePos.x += marginWide.x;

            // Subtitle font.
            subtitleFont.scale = new Vec2(scale);
            subtitleFont.spriteScale = subtitleFont.scale;
            subtitleFont.singleLine = true;

            float stFontHeight = subtitleFont.height;

            // Subtitle calculations.
            float subtitleMargin = m_subtitleLines > 0 ? 5f * scale : 0f;
            float subtitleWidth = subtitleFont.GetWidth(m_subtitle);
            float subtitleHeight = stFontHeight * m_subtitleLines;

            float maxWidth = Math.Max(titleWidth, subtitleWidth);

            // Maximum and minimum width.
            if (MinSize.x > -1f)
            {
                maxWidth = Math.Max(maxWidth - totalMarginX, MinSize.x * scale);
            }
            if (MaxSize.x > -1f)
            {
                maxWidth = Math.Min(maxWidth - totalMarginX, MaxSize.x * scale);
            }

            m_size.x = maxWidth + totalMarginX;

            Vec2 subtitlePos = titlePos;
            subtitlePos.y += subtitleMargin;
            subtitlePos.y += titleHeight;
            subtitlePos.x += maxWidth / 2f;
            subtitlePos.x -= subtitleWidth / 2f;

            // Center title.
            Vec2 titleOffset = new Vec2(0f, 0f);
            titleOffset.x += maxWidth / 2f;
            titleOffset.x -= titleWidth / 2f;

            // Slide animation.
            float progress = (float)m_animFrame / m_animDuration;
            progress *= 90f;
            progress = Maths.DegToRad(progress);
            progress = (float)Math.Sin(progress);

            float offsetX = 15f * scale;
            offsetX += margin.x * 2f;
            offsetX += marginWide.x;
            offsetX += maxWidth;
            offsetX += m_positionOffset.x;
            offsetX *= progress;

            titlePos.x -= offsetX;
            subtitlePos.x -= offsetX;
            
            // Calculate background rects.
            float totalHeight = titleHeight;
            totalHeight += subtitleHeight;
            totalHeight += subtitleMargin;

            // Maximum and minimum height.
            if (MinSize.y > -1f)
            {
                totalHeight = Math.Max(totalHeight - totalMarginY, MinSize.y * scale);
            }
            if (MaxSize.y > -1f)
            {
                totalHeight = Math.Min(totalHeight - totalMarginY, MaxSize.y * scale);
            }

            m_size.y = totalHeight + totalMarginY;

            Vec2 rectTL = titlePos;
            rectTL -= margin;
            Vec2 rectBR = titlePos;
            rectBR += margin;
            rectBR += new Vec2(maxWidth, totalHeight);

            // Set notification's position.
            m_basePosition = rectTL;

            // Set icon pos.
            Vec2 iconPos = titlePos;
            iconPos.x += maxWidth / 2f;
            iconPos.y -= tFontHeight;
            iconPos.y -= 5f * scale;
            iconPos += IconPosOffset;

            if (DecorationIcon != null)
            {
                Graphics.Draw(DecorationIcon, iconPos.x, iconPos.y, DepthTable.NOTIFICATION_TEXT);
            }

            // Draw title & subtitle.
            titleFont.Draw(m_title, titlePos + titleOffset, m_titleColor, DepthTable.NOTIFICATION_TEXT);
            subtitleFont.Draw(m_subtitle, subtitlePos, m_subtitleColor, DepthTable.NOTIFICATION_TEXT);

            // Draw background.
            Graphics.DrawRect(rectTL + marginWide, rectBR - marginWide, ColorTable.BACKGROUND, DepthTable.NOTIFICATION_BACKGROUND);
            Graphics.DrawRect(rectTL + marginHigh, rectBR - marginHigh, ColorTable.BACKGROUND, DepthTable.NOTIFICATION_BACKGROUND);

            // Title font termination.
            titleFont.scale = prevScaleT;
            titleFont.spriteScale = prevSprScaleT;
            titleFont.singleLine = prevSingleLineT;

            // Subtitle font termination.
            subtitleFont.scale = prevScaleST;
            subtitleFont.spriteScale = prevSprScaleST;
            subtitleFont.singleLine = prevSingleLineST;

            // Draw progress bar.
            Vec2 tl = TR;
            Vec2 br = BR;
            tl.x += 5f * scale;
            br.x += 10f * scale;

            Vec2 barMargin = new Vec2(1f * scale);

            // Draw progress bar outline.
            Graphics.DrawRect(tl - barMargin, br + barMargin, ColorTable.BACKGROUND, DepthTable.NOTIFICATION_BACKGROUND_OUTLINE, true, 0f);

            float yVal = (br.y - tl.y) * LifeNorm;
            yVal *= 4f;
            yVal = (float)Math.Floor(yVal);
            yVal /= 4f;

            tl.y += yVal;

            // Draw progress.
            Graphics.DrawRect(tl, br, ColorTable.PROGRESS_BAR, DepthTable.NOTIFICATION_BACKGROUND, true, 0f);

            // Debug circles.
            if (DevConsole.core.showCollision)
            {
                Graphics.DrawCircle(Center, 2f, Color.Red, 1f, DepthTable.MAX);
                Graphics.DrawCircle(TL, 2f, Color.Blue, 1f, DepthTable.MAX);
                Graphics.DrawCircle(TR, 2f, Color.Blue, 1f, DepthTable.MAX);
                Graphics.DrawCircle(BL, 2f, Color.Blue, 1f, DepthTable.MAX);
                Graphics.DrawCircle(BR, 2f, Color.Blue, 1f, DepthTable.MAX);
                Graphics.DrawCircle(iconPos, 2f, Color.Green, 1f, DepthTable.MAX);
            }
        }

        /// <inheritdoc/>
        public object Clone()
        {
            Notification notif = (Notification)MemberwiseClone();

            return notif;
        }
    }
}
    