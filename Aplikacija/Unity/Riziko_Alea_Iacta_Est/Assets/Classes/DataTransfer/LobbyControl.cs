using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class LobbyControl
    {
        public int lobbyId;
        public string host;
        public List<Player> players = new List<Player>();
        public string mapName;
        public int turnDuration;
        public string gameType;
        public string joinCode;

        public LobbyControl() { }
        public LobbyControl(int id, string h, List<Player> p, string mn, int td, string gt, string jc)
        {
            lobbyId = id;
            host = h;
            players = p;
            mapName = mn;
            turnDuration = td;
            gameType = gt;
            joinCode = jc;
        }
    }

}
