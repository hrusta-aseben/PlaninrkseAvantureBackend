namespace PlaninarskeAvantureBackend.ViewModels
{
    public class CampingEditVM
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        public int CijenaPoNoci { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public IFormFile CampSlika { get; set; }
        public string Lokacija { get; set; }
    }
}
