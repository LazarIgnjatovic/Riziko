using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Classes.DataTransfer
{
    public class LeaderboardTransfer
    {
        public string username=null;
        public int elo=0;
        public int position;

        public LeaderboardTransfer() { }
        public LeaderboardTransfer(int e,string u,int pos)
        {
            username = u;
            elo = e;
            position = pos;
        }
    }
}
