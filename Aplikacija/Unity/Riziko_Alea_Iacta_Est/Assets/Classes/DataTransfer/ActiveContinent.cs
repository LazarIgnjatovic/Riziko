using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class ActiveContinent
    {
        public string continentName;
        public int bonusTanks;
        public List<Territory> territories=new List<Territory>();

        public ActiveContinent() { }
    }
}
