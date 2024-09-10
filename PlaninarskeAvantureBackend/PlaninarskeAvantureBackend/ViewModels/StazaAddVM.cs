using Microsoft.AspNetCore.Http;

namespace PlaninarskeAvantureBackend.ViewModels
{
    public class StazaAddVM
    {
        public string Naziv { get; set; }
        public string Tezina { get; set; }
        public int Duzina { get; set; }
        public int NadmorskaVisina { get; set; }
        public string Opis { get; set; }
        public string Lokacija { get; set; }
        public float pocetakX { get; set; }
        public float pocetakY { get; set; }
        public float krajX { get; set; }
        public float krajY { get; set; }
        public int PlaninaId { get; set; }
        public IFormFile SlikaStaze { get; set; }
    }
}
