
namespace FIT_Api_Example.Modul.Data
{
    public class Korisnik
    {
        public int Id { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public DateTime DatumRodjenja { get; set; }
        public string Drzava { get; set; }
        public string Lozinka { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public string? SlikaKorisnika { get; set; }
        public bool jelPotvrdjen { get; set; } = false;
        public bool TWOFA { get; set; } = false;
        public bool isAktivan { get; set; } = true;
    }
}
