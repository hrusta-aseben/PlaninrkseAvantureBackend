using FIT_Api_Example.Modul.Data;

namespace PlaninarskeAvantureBackend.ViewModels
{
    public class PostGetKomentarVM
    {
        public int Id { get; set; }
        public Korisnik Korisnik { get; set; }
        public string Tekst { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public int LikeCounter { get; set; }

    }
}
