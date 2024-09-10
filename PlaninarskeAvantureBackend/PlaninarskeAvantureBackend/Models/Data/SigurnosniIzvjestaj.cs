using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class SigurnosniIzvjestaj
    {
        public int Id { get; set; }
        public DateTime DatumIzvjestaja { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public string Opis { get; set; }
        public string Ostecenja { get; set; }
        public string Povreda { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        [ForeignKey(nameof(Dogadjaj))]
        public int DogadjajID { get; set; }
        public Dogadjaj Dogadjaj{ get; set; }
        public string SlikaIzvjestaj{ get; set; }
    }
}
