using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Staza.Validators
{
    public class StazaAddValidator: AbstractValidator<StazaAddVM>
    {
        public StazaAddValidator()
        {
            RuleFor(staza => staza.Naziv).NotNull();
            RuleFor(staza => staza.Tezina).NotNull();
            RuleFor(staza => staza.Duzina).NotNull();
            RuleFor(staza => staza.NadmorskaVisina).NotNull();
            RuleFor(staza => staza.Opis).NotNull();
            RuleFor(staza => staza.Lokacija).NotNull();
            RuleFor(staza => staza.pocetakX).NotNull();
            RuleFor(staza => staza.pocetakY).NotNull();
            RuleFor(staza => staza.krajX).NotNull();
            RuleFor(staza => staza.krajY).NotNull();

        }
    }
}
