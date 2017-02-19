using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pasvaldibas.Web.Models
{
    public class Deputats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DeputatsId { get; set; }
        public string Name { get; set; }
        public List<Apmeklejums> ApmekletasSedes { get; set; }
        public virtual Pasvaldiba Pasvaldiba { get; set; }
    }
}