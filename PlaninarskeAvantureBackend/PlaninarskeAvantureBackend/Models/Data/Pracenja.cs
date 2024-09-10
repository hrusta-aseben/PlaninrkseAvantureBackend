using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Pracenja
    {
        public int Id { get; set; }
        [ForeignKey(nameof(KorisnikUser))]
        public int KorisnikUserId { get; set; }
        public Korisnik KorisnikUser { get; set; }
        [ForeignKey(nameof(KorisnikZapracen))]
        public int KorsnikZapracenId { get; set; }
        public Korisnik KorisnikZapracen { get; set; }

    }
}
