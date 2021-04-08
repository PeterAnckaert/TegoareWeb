using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TegoareWeb.Models
{
    public class Inschrijving
    {
        [Key]
        public Guid Id { get; set; }

        [Column("lid_id")]
        public Guid Id_Lid { get; set; }
        [ForeignKey("Id_Lid")]
        public Lid Lid { get; set; }

        [Column("activiteit_id")]
        public Guid Id_Activiteit { get; set; }
        [ForeignKey("Id_Activiteit")]
        public Activiteit Activiteit { get; set; }

    }
}
