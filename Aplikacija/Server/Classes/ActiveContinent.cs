using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class ActiveContinent
    {
        public string continentName;
        public int bonusTanks;
        public List<Territory> territories = new List<Territory>();
        public ActiveContinent() { }
        public ActiveContinent(Continent continent)
        {
            continentName = continent.ContinentName;
            bonusTanks = continent.BonusTanks;
            List<Territory> neighs = new List<Territory>();
            foreach(Province province in continent.Provinces)
            {
                /* foreach(Province neighbors in province.ConnectedTo)
                 {
                     neighs.Add(new Territory(neighbors.ProvinceName));
                 }
                 territories.Add(new Territory(province.ProvinceName,neighs));
                 neighs.Clear();*/
                foreach(Province neighbors in province.ConnectedTo)
                {
                    neighs.Add(new Territory(neighbors.ProvinceName));
                }
                territories.Add(new Territory(province.ProvinceName,neighs));
                neighs.Clear();
            }
        }
    }
}
