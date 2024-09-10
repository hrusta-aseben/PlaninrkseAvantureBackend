using FIT_Api_Example.Modul.Data;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class Staza
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Tezina { get; set; }
        public int Duzina { get; set; }
        public int NadmorskaVisina { get; set; }
        public string Opis { get; set; }
        public string Lokacija { get; set; }
        public float pocetakX { get; set; }
        public float pocetakY { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik{ get; set; }
        public DateTime VrijemeKreiranja{ get; set; }
        public double Ocjena { get; set; } = 0;
        [ForeignKey(nameof(Planina))]
        public int PlaninaId { get; set; }
        public Planina Planina{ get; set; }
        public float krajX { get; set; }
        public float krajY { get; set; }
        public string SlikaStaze{ get; set; }
    }
}
