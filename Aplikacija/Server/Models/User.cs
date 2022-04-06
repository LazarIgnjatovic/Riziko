using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Server.Models
{
    public class User : IdentityUser
    {
        public virtual List<Game> GamesPlayed { get; set; }
        public User()
        {
            GamesPlayed = new List<Game>();
        }
    }
}
