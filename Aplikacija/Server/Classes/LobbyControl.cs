using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class LobbyControl:IDisposable
    {
        public int lobbyId;
        public string host;
        public List<Player> players = new List<Player>();
        public string mapName;
        public int turnDuration;
        public string gameType;
        public string joinCode;
        public List<Player> disconnectedPlayers = new List<Player>();
        private bool disposedValue;

        public LobbyControl() { }
        public LobbyControl(int id,string h,string gt)
        {
            lobbyId = id;
            host = h;
            gameType = gt;
            //default parametri
            mapName = "World";
            turnDuration = 60;
            joinCode = null;
        }
        public LobbyControl(int id, string h, string gt,string jc)
        {
            lobbyId = id;
            host = h;
            gameType = gt;
            //default parametri
            mapName = "World";
            turnDuration = 60;
            joinCode = jc;
        }
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~LobbyControl()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}