using FIT_Api_Example.Modul.Data;
using PlaninarskeAvantureBackend.Modul.Data;

namespace PlaninarskeAvantureBackend.ViewModels
{
    public class KorisnikGetPostovi
    {
        public int Id { get; set; }
        public Korisnik Korisnik { get; set; }
        public List<Post> Postovi{ get; set; }
    }
}
