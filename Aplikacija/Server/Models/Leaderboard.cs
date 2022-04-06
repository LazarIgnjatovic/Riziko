using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Leaderboard
    {
        [Key]
        public int Id { get; set; }
        public int ELO { get; set; }
        public int GamesPlayed { get; set; }
        public User User { get; set; }
    }
}
