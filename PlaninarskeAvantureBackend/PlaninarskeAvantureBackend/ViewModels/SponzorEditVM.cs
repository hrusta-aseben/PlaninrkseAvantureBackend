namespace PlaninarskeAvantureBackend.ViewModels
{
    public class SponzorEditVM
    {
        public int SponzorId { get; set; }
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        public IFormFile SponzorSlika { get; set; }

    }
}
