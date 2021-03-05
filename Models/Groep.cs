﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Groep
    {
        public Guid Id { get; set; }
        public string Rol { get; set; }
        public string Omschrijving { get; set; }
        public bool Dubbele_Relatie { get; set; }
        public IList<Relatie> Relaties { get; set; }
    }
}
