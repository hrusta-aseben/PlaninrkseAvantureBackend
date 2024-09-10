using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class PutopisStaze
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Putopis))]
        public int PutopisId { get; set; }
        public Putopis Putopis{ get; set; }
        [ForeignKey(nameof(Staza))]
        public int StazaId { get; set; }
        public Staza Staza{ get; set; }

    }
}
