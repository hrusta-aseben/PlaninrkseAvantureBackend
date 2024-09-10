namespace PlaninarskeAvantureBackend.ViewModels
{
    public class CampingAddVM
    {
        public string Naziv { get; set; }
        public string Tip { get; set; }
        public string Kontakt { get; set; }
        public int CijenaPoNoci { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public IFormFile SlikaCamp { get; set; }
        public string Lokacija { get; set; }
    }
}
