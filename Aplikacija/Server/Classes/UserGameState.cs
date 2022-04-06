using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class UserGameState
    {
        public string gameId;
        public List<Player> players=new List<Player>();
        public string winCond;
        public List<Card> Cards=new List<Card>();
        public ActiveMap gameMap;
        public int turnDuration;
        public Player onTurn;
        public string phase;
        public int draftTanks;
        public UserGameState()
        {

        }
        public UserGameState(string gid, List<Player> p, List<Card> c, ActiveMap gm, int td, Player ot,string ph,int dt)
        {
            gameId = gid;
            players = p;
            Cards = c;
            gameMap = gm;
            turnDuration = td;
            onTurn = ot;
            phase = ph;
            draftTanks = dt;
        }
        public UserGameState(string gid, List<Player> p, string wc, List<Card> c, ActiveMap gm, int td, Player ot, string ph,int dt) // init
        {
            gameId = gid;
            players = p;
            winCond = wc;
            Cards = c;
            gameMap = gm;
            turnDuration = td;
            onTurn = ot;
            phase = ph;
            draftTanks = dt;
        }
    }
}
