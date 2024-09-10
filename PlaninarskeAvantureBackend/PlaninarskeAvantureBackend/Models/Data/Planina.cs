using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Planina
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string NajvisiVrh { get; set; }
        public int Visina { get; set; }
        public string OpisPlanine { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik{ get; set; }
    }
}
