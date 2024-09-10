using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Izvjestaj.IzvjestajValidator
{
    public class IzvjestajValidate:AbstractValidator<IzvjestajAddVM>
    {
        public IzvjestajValidate()
        {
            RuleFor(izvjestaj => izvjestaj.x).NotNull();
            RuleFor(izvjestaj => izvjestaj.y).NotNull();
            RuleFor(izvjestaj => izvjestaj.Opis).NotNull();
            RuleFor(izvjestaj => izvjestaj.Ostecenja).NotNull();
            RuleFor(izvjestaj => izvjestaj.Povreda).NotNull();
        }
    }
}
