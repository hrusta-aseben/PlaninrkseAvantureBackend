using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Dogadjaj
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public DateTime VrijemePocetka { get; set; }
        public DateTime VrijemeZavrsetka { get; set; }
        public string Lokacija { get; set; }
        public float xKordinata { get; set; }
        public float yKordinata { get; set; }
        public string Opis { get; set; }
        public int BrojPlaninara { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public DateTime VrijemeKreiranja { get; set; }
        public string SlikaDogadjaja{ get; set; }

    }
}
