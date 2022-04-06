using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Settings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Avatar { get; set; }
        public float Sound { get; set; } 
        public string PrefColor { get; set; }
        public bool ActionConfirm { get; set; }
        public User User { get; set; }
    }
}
