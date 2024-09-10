namespace PlaninarskeAvantureBackend.ViewModels
{
    public class SponzorAddVM
    {
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        public int DogadjajId { get; set; }
        public IFormFile SponzorSlika { get; set; }
    }
}
