using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class OcjenaStaze
    {
        public int Id { get; set; }
        public string Komentar { get; set; }
        public int Ocjena { get; set; }
        public string Kvalitet { get; set; }
        [ForeignKey(nameof(Staza))]
        public int StazaId { get; set; }
        public Staza Staza { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }

    }
}
