using FIT_Api_Example.Modul.Data;
using FIT_Api_Example.ViewModels;
using FluentValidation;
using PlaninarskeAvantureBackend.Controllers.Korisnik.VM;

namespace PlaninarskeAvantureBackend.Controllers.Korisnik.Validators
{
    public class KorisnikRegistracijaValidator : AbstractValidator<KorisnikAddVM>
    {
        public KorisnikRegistracijaValidator()
        {
            RuleFor(korisnik => korisnik.Ime).NotNull();
            RuleFor(korisnik => korisnik.Prezime).NotNull();
            RuleFor(korisnik => korisnik.Email).NotNull().EmailAddress();
            RuleFor(korisnik => korisnik.DatumRodjenja).NotNull();
            RuleFor(korisnik => korisnik.Lozinka).NotNull();
            RuleFor(korisnik => korisnik.PotvrdaLozinke).NotNull();
            RuleFor(korisnik => korisnik.Drzava).NotNull();
        }

    }
}
