using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Drustvo.Validators
{
    public class DrustvoCreateValidator: AbstractValidator<KreirajDrustvoVM>
    {
        public DrustvoCreateValidator() {
            RuleFor(drustvo => drustvo.Naziv).NotNull();
            RuleFor(drustvo=>drustvo.Opis).NotNull();
            RuleFor(drustvo=>drustvo.Adresa).NotNull();
            RuleFor(drustvo => drustvo.Kontakt).NotNull();
        }

    }
}
