using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes.DataTransfer
{
    public class UserGameState
    {
        public string gameId;
        public List<Player> players=new List<Player>();
        public string winCond;
        public List<Card> Cards = new List<Card>();
        public ActiveMap gameMap;
        public int turnDuration;
        public Player onTurn;
        public string phase;
        public int draftTanks;
        public UserGameState()
        {

        }

        public Territory FindTerritory(string name)
        {
            foreach (ActiveContinent c in gameMap.continents)
            {
                foreach (Territory t in c.territories)
                {
                    if (t.name == name)
                        return t;
                }
            }
            return null;
        }
    }
}
