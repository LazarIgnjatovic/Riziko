using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class Territory
    {
        public string name;
        public Player currentHolder;
        public int tanks;
        public List<Territory> neighbors = new List<Territory>();
        public Territory() { }
        public Territory(string n)
        {
            name = n;
        }
        public Territory(string n, List<Territory> neighs)
        {
            name = n;
            foreach(Territory terr in neighs)
                neighbors.Add(terr);
        }
        public Territory(string n, Player ch, int t)
        {
            name = n;
            currentHolder = ch;
            tanks = t;
        }
    }
}
