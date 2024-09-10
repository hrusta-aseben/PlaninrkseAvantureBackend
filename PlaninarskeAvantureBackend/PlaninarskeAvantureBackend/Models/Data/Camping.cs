using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Camping
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        public int CijenaPoNoci { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public string CampSlika { get; set; }
        public string Lokacija { get; set; }
    }
}
