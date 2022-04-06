using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Classes.DataTransfer
{
    public class Card
    {
        public string territoryName;
        public string type;
        public Player holder;
        public int valid = 1;

        public Card() { }
    }
}
