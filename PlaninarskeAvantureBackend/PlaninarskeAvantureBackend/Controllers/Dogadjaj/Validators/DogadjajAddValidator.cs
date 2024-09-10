using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Dogadjaj.Validators
{
    public class DogadjajAddValidator:AbstractValidator<DogadjajAddVM>
    {
        public DogadjajAddValidator()
        {
            RuleFor(dogadjaj => dogadjaj.Naziv).NotNull();
            RuleFor(dogadjaj => dogadjaj.VrijemePocetka).NotNull();
            RuleFor(dogadjaj => dogadjaj.VrijemeZavrsetka).NotNull();
            RuleFor(dogadjaj => dogadjaj.Lokacija).NotNull();
            RuleFor(dogadjaj => dogadjaj.xKordinata).NotNull();
            RuleFor(dogadjaj => dogadjaj.yKordinata).NotNull();
            RuleFor(dogadjaj => dogadjaj.Opis).NotNull();
        }
    }
}
