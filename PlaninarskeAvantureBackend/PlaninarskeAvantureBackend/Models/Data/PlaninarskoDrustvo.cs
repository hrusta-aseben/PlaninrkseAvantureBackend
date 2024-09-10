using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class PlaninarskoDrustvo
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public string Kontakt { get; set; }
        public string Opis { get; set; }
        [ForeignKey(nameof(KorisnikAdmin))]
        public int KorisnikId { get; set; }
        public Korisnik KorisnikAdmin { get; set; }
        public string DrustvoSlika { get; set; }
    }
}
