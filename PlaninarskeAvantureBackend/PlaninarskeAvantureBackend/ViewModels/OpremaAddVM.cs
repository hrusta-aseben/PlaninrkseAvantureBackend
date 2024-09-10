namespace PlaninarskeAvantureBackend.ViewModels
{
    public class OpremaAddVM
    {
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Proizvodjac { get; set; }
        public string Kategorija { get; set; }
        public string Recenzija { get; set; }
        public IFormFile SlikaOpreme { get; set; }
    }
}
