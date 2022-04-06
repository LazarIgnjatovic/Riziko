using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class ActiveMap
    {
        public string mapName;
        public List<ActiveContinent> continents = new List<ActiveContinent>();
        public ActiveMap() { }
        public ActiveMap(Map m)
        {
            mapName = m.MapName;
            foreach(Continent continent in m.Continents)
            {
                continents.Add(new ActiveContinent(continent));
            }
        }
    }
}
