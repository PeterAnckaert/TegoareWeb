using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TegoareWeb.Models
{
    public class Relatie
    {
        public Guid Id { get; set; }

        [Column("lid1_id")]
        public Guid Id_Lid1 { get; set; }
        [ForeignKey("Id_Lid1")]
        public Lid Lid1 { get; set; }

        [Column("groep_id")]
        public Guid Id_Groep { get; set; }
        [ForeignKey("Id_Groep")]
        public Groep Groep { get; set; }

        [Column("lid2_id")]
        public Guid? Id_Lid2 { get; set; }
        [ForeignKey("Id_Lid2")]
        public Lid Lid2 { get; set; }
    }
}
