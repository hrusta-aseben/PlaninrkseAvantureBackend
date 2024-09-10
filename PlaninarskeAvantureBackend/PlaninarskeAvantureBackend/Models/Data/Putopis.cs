using FIT_Api_Example.Modul.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Putopis
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public DateTime DatumObjavljivanja { get; set; }
        public string Opis { get; set; }
    }
  
}
