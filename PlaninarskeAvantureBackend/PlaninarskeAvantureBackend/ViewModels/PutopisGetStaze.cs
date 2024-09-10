using PlaninarskeAvantureBackend.Models.Data;

namespace PlaninarskeAvantureBackend.ViewModels
{
    public class PutopisGetStaze
    {
        public int Id { get; set; }
        public Putopis Putopis { get; set; }
        public List<Staza>? Staza { get; set; }
    }
}
