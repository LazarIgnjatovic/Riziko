using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Classes
{
    public class Player
    {
        public string username;
        public string color;
        public int elo;
        public int avatar;
        public Player()
        {

        }
        public Player(string un, string c, int e, int a)
        {
            username = un;
            color = c;
            elo = e;
            avatar = a;
        }
    }
}
