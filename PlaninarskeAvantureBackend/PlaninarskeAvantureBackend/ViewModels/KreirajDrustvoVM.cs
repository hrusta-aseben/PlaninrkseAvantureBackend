namespace PlaninarskeAvantureBackend.ViewModels
{
    public class KreirajDrustvoVM
    {
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public string Kontakt { get; set; }
        public string Opis { get; set; }
        public IFormFile SlikaDrustva { get; set; }
    }
}
