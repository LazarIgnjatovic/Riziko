using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Continent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ContinentName { get; set; }
        public int BonusTanks { get; set; }
        public virtual List<Province> Provinces { get; set; }
        public Continent()
        {
            Provinces = new List<Province>();
        }
    }
}
