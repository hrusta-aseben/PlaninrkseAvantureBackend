namespace PlaninarskeAvantureBackend.ViewModels
{
    public class DogadjajUrediVM
    {
        public int DogadjajID { get; set; }
        public string Naziv { get; set; }
        public DateTime VrijemePocetka { get; set; }
        public DateTime VrijemeZavrsetka { get; set; }
        public string Lokacija { get; set; }
        public float xKordinata { get; set; }
        public float yKordinata { get; set; }
        public string Opis { get; set; }
        public int BrojPlaninara { get; set; }
        public IFormFile Slika{ get; set; }
    }
}
