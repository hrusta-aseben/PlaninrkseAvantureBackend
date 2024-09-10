using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Sponzor
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        [ForeignKey(nameof(Dogadjaj))]
        public int DogadjajId { get; set; }
        public Dogadjaj Dogadjaj { get; set; }

        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId{ get; set; }
        public Korisnik Korisnik{ get; set; }
        public string SponzorSlika { get; set; }
    }

}
