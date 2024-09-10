namespace PlaninarskeAvantureBackend.ViewModels
{
    public class SigurnosniRizikUrediVM
    {
        public int rizikId { get; set; }
        public string tipRizika { get; set; }
        public string opisRizika { get; set; }
        public string lokacijaRizika { get; set; }
        public IFormFile RizikSlika { get; set; }
    }
}
