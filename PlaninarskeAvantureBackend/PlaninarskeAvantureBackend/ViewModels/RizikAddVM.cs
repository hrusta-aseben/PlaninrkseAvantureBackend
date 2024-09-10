namespace PlaninarskeAvantureBackend.ViewModels
{
    public class RizikAddVM
    {
        public string tipRizika { get; set; }
        public string opisRizika { get; set; }
        public string lokacijaRizika { get; set; }
        public int StazaId { get; set; }
        public IFormFile RizikSlika { get; set; }
    }
}
