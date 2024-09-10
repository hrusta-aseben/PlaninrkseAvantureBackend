namespace PlaninarskeAvantureBackend.ViewModels
{
    public class PutopisEditVM
    {
        public int Id { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public List<int>? Staza { get; set; }
    }
}
