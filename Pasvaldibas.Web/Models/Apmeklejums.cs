using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasvaldibas.Web.Models
{
    public class Apmeklejums
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ApmeklejumsId { get; set; }

        public string ApmeklejumaNr { get; set; }

        public bool Apmekleja { get; set; }
        
        public string NeapmeklesanasIemesls { get; set; }

        public DateTime Datums { get; set; }

        public virtual Deputats Deputats { get; set; }
    }
}