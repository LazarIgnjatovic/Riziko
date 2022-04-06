using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Server.Models;

namespace Server.Classes
{
    public class Card
    {
        public string territoryName;
        public string type;
        public Player holder;
        public int valid=1;
        public Card() 
        {
            
        }
        public Card(string t, string ty)
        {
            territoryName = t;
            type = ty;
        }
        public void AttachHolder(Player h)
        {
            holder = h;
            valid = 0;
        }
        public void DettachHolder()
        {
            holder = null;
            valid = 1;
        }
    }
}
