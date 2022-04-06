using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Classes
{
    public class LobbyMaster
    {
        public List<LobbyControl> activeLobbies = new List<LobbyControl>();

        internal void DestroyMe(LobbyControl lobbyControl)
        {
            activeLobbies.Remove(lobbyControl);
            lobbyControl.Dispose();
        }
    }
}
