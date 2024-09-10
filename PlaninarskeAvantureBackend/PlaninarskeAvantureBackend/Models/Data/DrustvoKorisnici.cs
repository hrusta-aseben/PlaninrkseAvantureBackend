using FIT_Api_Example.Modul.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class DrustvoKorisnici
    {
        public int Id { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        [ForeignKey(nameof(PlaninarskoDrustvo))]
        public int PlaninarskoDrustvoID { get; set; }
        public PlaninarskoDrustvo PlaninarskoDrustvo { get; set; }
    }
}
