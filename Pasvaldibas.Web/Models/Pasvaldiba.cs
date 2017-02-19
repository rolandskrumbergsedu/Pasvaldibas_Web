﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasvaldibas.Web.Models
{
    public class Pasvaldiba
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PasvaldibaId { get; set; }

        public string Code { get; set; }

        public string CodeNr { get; set; }

        public string Name { get; set; }

        public virtual List<Deputats> Deputati { get; set; }
    }
}
