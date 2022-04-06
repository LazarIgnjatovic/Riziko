using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Province
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string ProvinceName { get; set; }
        public virtual List<Province> Connected { get; set; }
        public virtual List<Province> ConnectedTo { get; set; }
        public Province()
        {
            Connected = new List<Province>();
            ConnectedTo = new List<Province>();
        }
    }
}
