namespace PlaninarskeAvantureBackend.ViewModels
{
    public class KorisnikUrediVM
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Drzava { get; set; }
        public string PotvrdaLozinke { get; set; }
        public IFormFile SlikaKorisnika { get; set; }
        public bool TWOFA { get; set; }
    }
}
