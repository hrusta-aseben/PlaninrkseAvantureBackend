using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Sponzor.SponzorValidators
{
    public class SponzorAddValidator:AbstractValidator<SponzorAddVM>
    {
        public SponzorAddValidator()
        {
            RuleFor(sponzor => sponzor.Naziv).NotNull();
            RuleFor(sponzor => sponzor.Kontakt).NotNull();
            RuleFor(sponzor => sponzor.Tip).NotNull();
        }
    }
}
