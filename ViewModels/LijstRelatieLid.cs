﻿using System;
using System.Collections.Generic;
using TegoareWeb.Models;

namespace TegoareWeb.ViewModels
{
    public class LijstRelatieLid
    {
        public Guid LidId { get; set; }
        public IEnumerable<Relatie> Relaties { get; set; }
        public bool Knop { get; set; } = false;
    }
}
