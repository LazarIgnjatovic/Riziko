using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsFriendly { get; set; }
        public bool IsRanked { get; set; }
        public bool IsCasual { get; set; }
        public User Winner { get; set; }
        public Map Map { get; set; }
        public virtual List<User> Players { get; set; }

        public Game()
        {
            Players = new List<User>();
        }
        
    }
}
