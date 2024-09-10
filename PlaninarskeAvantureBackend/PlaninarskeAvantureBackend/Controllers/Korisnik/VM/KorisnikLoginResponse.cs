namespace PlaninarskeAvantureBackend.Controllers.Korisnik.VM
{
    public class KorisnikLoginResponse
    {
        public FIT_Api_Example.Modul.Data.Korisnik Korisnik { get; set; }
        public string Token { get; set; }
    }
}
