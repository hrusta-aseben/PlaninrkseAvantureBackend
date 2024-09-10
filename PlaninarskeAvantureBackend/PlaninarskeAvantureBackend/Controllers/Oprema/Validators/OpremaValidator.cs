using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Oprema.Validators
{
    public class OpremaValidator:AbstractValidator<OpremaAddVM>
    {
        public OpremaValidator() {
            RuleFor(oprema => oprema.Naziv).NotNull();
            RuleFor(oprema => oprema.Kategorija).NotNull();
        }

    }
}
