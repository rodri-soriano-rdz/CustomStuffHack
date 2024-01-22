using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame.CustomStuffHack.Utility;

namespace DuckGame.CustomStuffHack.Images
{
    internal static class TeamSynchronizer
    {
        private static readonly Queue<Tuple<Team, NetworkConnection>> s_syncQueue = new Queue<Tuple<Team, NetworkConnection>>();
        private static readonly Queue<NetworkConnection> s_connectionQueue = new Queue<NetworkConnection>();
        private static readonly IDictionary<int, List<string>> s_syncHistory = new Dictionary<int, List<string>>();

        public static void PerformSynchronizations()
        {
            if (!DuckNetwork.active)
            {
                Reset();
                return;
            }

            if (s_connectionQueue.Count > 0)
            {
                NetworkConnection con = s_connectionQueue.Dequeue();

                foreach (BaseImage img in ImageManager.ActiveImages)
                {
                    img.Sync(con);
                }
            }

            while (s_syncQueue.Count > 0)
            {
                Tuple<Team, NetworkConnection> tuple = s_syncQueue.Dequeue();

                Team team = tuple.Item1;
                NetworkConnection connection = tuple.Item2;

                TrySendTeamToConnection(team, connection);
            }
        }

        public static void EnqueueTeamForSynchronization(Team team, NetworkConnection connetion = null)
        {
            if (connetion == null)
            {
                foreach (NetworkConnection con in Network.connections)
                {
                    var tuple = Tuple.Create(team, con);
                    s_syncQueue.Enqueue(tuple);
                }
            }
            else
            {
                var tuple = Tuple.Create(team, connetion);
                s_syncQueue.Enqueue(tuple);
            }
        }

        public static bool TrySendTeamToConnection(Team team, NetworkConnection connection)
        {
            string teamName = team.name;
            int connectionID = connection.connectionID;

            var syncMsg = new NMSpecialHat(team, null);

            List<string> teamsSynced;
            bool found = s_syncHistory.TryGetValue(connectionID, out teamsSynced);

            if (!found)
            {
                teamsSynced = new List<string>();
                s_syncHistory.Add(connectionID, teamsSynced);
            }

            if (teamsSynced.Contains(teamName))
            {
                return false;
            }

            Send.Message(syncMsg, connection);
            teamsSynced.Add(teamName);

            SFX.Play("demo");

            return true;
        }

        public static void EnqueueConnectionForSync(NetworkConnection connection)
        {
            s_connectionQueue.Enqueue(connection);
        }

        public static void ClearSyncHistoryForConnection(NetworkConnection connection)
        {
            if (s_syncHistory.ContainsKey(connection.connectionID))
            {
                s_syncHistory[connection.connectionID].Clear();
            }
        }

        private static void Reset()
        {
            s_syncQueue.Clear();
            s_connectionQueue.Clear();
            s_syncHistory.Clear();
        }
    }
}
