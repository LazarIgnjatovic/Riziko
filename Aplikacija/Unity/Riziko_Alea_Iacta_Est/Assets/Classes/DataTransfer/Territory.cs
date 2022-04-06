using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assets.Classes
{
    public class Territory
    {
        public string name;
        public Player currentHolder;
        public int tanks;
        public List<Territory> neighbors = new List<Territory>();

        public Territory() { }
    }
}
