using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class DogadjajKorisnici
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        [ForeignKey(nameof(Dogadjaj))]
        public int DogadjajID { get; set; }
        public Dogadjaj Dogadjaj { get; set; }
    }
}
