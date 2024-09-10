using System.ComponentModel.DataAnnotations.Schema;

namespace PlaninarskeAvantureBackend.Models.Data
{
    public class SigurnosniRizik
    {
        public int Id { get; set; }
        public string tipRizika { get; set; }
        public string opisRizika { get; set; }
        public string lokacijaRizika { get; set; }
        [ForeignKey(nameof(Staza))]
        public int StazaId { get; set; }
        public Staza Staza { get; set; }
        public string RizikSlika { get; set; }
    }
}
