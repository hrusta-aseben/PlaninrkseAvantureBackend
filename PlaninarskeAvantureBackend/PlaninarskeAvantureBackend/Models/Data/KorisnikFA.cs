using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class KorisnikFA
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public string Kod { get; set; } = "";
        public DateTime DatumIsteka { get; set; }
        public bool isAktivan { get; set; } = false;
    }
}
