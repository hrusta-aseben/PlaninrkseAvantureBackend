using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Modul.Data
{
    public class Komentar
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        [ForeignKey(nameof(Post))]
        public int PostId { get; set; }
        public Post Post{ get; set; }
        public string TekstKomentara { get; set; }
        public int LikeCounter { get; set; }
        public DateTime DatumKreiranja { get; set; }
        
        public List<KomentarLajkovi> Lajkovi { get; set; }
    }
}
