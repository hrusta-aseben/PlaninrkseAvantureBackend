namespace PlaninarskeAvantureBackend.ViewModels
{
    public class StazaUrediVM
    {
        public int StazaID { get; set; }
        public string Naziv { get; set; }
        public string Tezina { get; set; }
        public int Duzina { get; set; }
        public int NadmorskaVisina { get; set; }
        public string Opis { get; set; }
        public string Lokacija { get; set; }
        public float pocetakX { get; set; }
        public float pocetakY{ get; set; }
        public float krajX { get; set; }
        public float krajY { get; set; }
        public IFormFile Slika { get; set; }

    }
}
