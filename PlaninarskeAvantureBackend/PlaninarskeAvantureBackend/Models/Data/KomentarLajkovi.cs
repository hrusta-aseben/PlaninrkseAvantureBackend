using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Modul.Data
{
    public class KomentarLajkovi
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        [ForeignKey(nameof(Komentar))]
        public int KomentarID { get; set; }
        public Komentar Komentar{ get; set; }
    }
}
