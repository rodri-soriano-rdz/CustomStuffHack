using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DuckGame.CustomShitHack.UI
{
    internal static class NotificationManager
    {
        public const float UI_SCALE = 0.5f;

        private static readonly Queue<Notification> s_notifQueue = new Queue<Notification>();
        private static Notification s_activeNotif;

        public static void EnqueueNotification(Notification notification)
        {
            if (notification == null) return;

            s_notifQueue.Enqueue(notification);
        }

        public static Notification EnqueueNotification(string title, string subtitle, uint duration, Sprite icon = null)
        {
            var notif = new Notification(title, subtitle, duration)
            {
                DecorationIcon = icon
            };

            EnqueueNotification(notif);
            return notif;
        }

        public static PushNotification EnqueuePushNotification(string title, string subtitle, uint duration, string actionText, PushNotification.PressAction action = null, Sprite icon = null)
        {
            var notif = new PushNotification(title, subtitle, duration, actionText, action)
            {
                DecorationIcon = icon
            };

            EnqueueNotification(notif);
            return notif;
        }

        public static void Clear()
        {
            s_notifQueue.Clear();
            s_activeNotif = null;
        }

        public static void MoveNext(bool force = true)
        {
            if (s_notifQueue.Count > 0)
            {
                // Terminate current notification, if any.
                if (s_activeNotif != null)
                {
                    if (!force)
                    {
                        s_activeNotif.SetSlideSpeed(5);
                    }
                    s_activeNotif.TryTerminate(!force);
                }

                if (force)
                {
                    // Take next notification in queue.
                    s_activeNotif = s_notifQueue.Dequeue();

                    if (s_activeNotif != null)
                    {
                        s_activeNotif.TryInitialize(true, true);
                    }
                }
            }
            else
            {
                s_activeNotif = null;
            }
        }

        public static void Update()
        {
            if (s_activeNotif != null)
            {
                s_activeNotif.Update();

                // Move to next notification if active notif has finished.
                if (!s_activeNotif.Active)
                {
                    MoveNext(true);
                }
            }
            else
            {
                MoveNext(true);
            }
        }

        public static void OnDraw(Layer l)
        {
            if (s_activeNotif != null)
            {
                s_activeNotif.Draw();
                
                if (DevConsole.showCollision)
                {
                    Graphics.DrawString($"LIFE: {s_activeNotif.LifeNorm}", new Vec2(10f, 5f), Color.Red, scale: 0.5f);
                    Graphics.DrawString($"ANIM: {s_activeNotif.AnimationNorm}", new Vec2(10f, 15f), Color.Orange, scale: 0.5f);
                    Graphics.DrawString($"ACTIVE: {s_activeNotif.Active}", new Vec2(10f, 25f), Color.Green, scale: 0.5f);
                    Graphics.DrawString($"SIZE: {s_activeNotif.Size}", new Vec2(10f, 35f), Color.Purple, scale: 0.5f);
                    Graphics.DrawString($"MIN SIZE: {s_activeNotif.MaxSize}", new Vec2(10f, 45f), Color.Purple, scale: 0.5f);
                    Graphics.DrawString($"MAX SIZE: {s_activeNotif.MinSize}", new Vec2(10f, 55f), Color.Purple, scale: 0.5f);
                    Graphics.DrawString($"QUEUE SIZE: {s_notifQueue.Count()}", new Vec2(10f, 65f), Color.CadetBlue, scale: 0.5f);
                    Graphics.DrawRect(new Vec2(0f, 0f), new Vec2(100f, 75f), Color.Black * 0.75f, -0.1f, true);
                }
            }
        }
    }
}
