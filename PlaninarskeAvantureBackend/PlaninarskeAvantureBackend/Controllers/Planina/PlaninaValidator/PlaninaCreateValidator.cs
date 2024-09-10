using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Planina.PlaninaValidator
{
    public class PlaninaCreateValidator:AbstractValidator<PlaninaAddVM>
    {
        public PlaninaCreateValidator() {
            RuleFor(planina => planina.Naziv).NotNull();
            RuleFor(planina => planina.NajvisiVrh).NotNull();
            RuleFor(planina => planina.Visina).NotNull();
            RuleFor(planina => planina.Opis).NotNull();
        }
    }
}
