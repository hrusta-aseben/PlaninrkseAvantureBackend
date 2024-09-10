namespace PlaninarskeAvantureBackend.ViewModels
{
    public class DrustvoUrediVM
    {
        public int drustvoId { get; set; }
        public string Naziv { get; set; }
        public string Adresa { get; set; }
        public string Kontakt { get; set; }
        public string Opis { get; set; }
        public IFormFile SlikaDrustva { get; set; }

    }
}
