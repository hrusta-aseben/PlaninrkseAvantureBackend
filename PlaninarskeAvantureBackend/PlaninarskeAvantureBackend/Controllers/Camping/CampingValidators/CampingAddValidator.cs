using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Camping.CampingValidators
{
    public class CampingAddValidator:AbstractValidator<CampingAddVM>
    {
        public CampingAddValidator()
        {
            RuleFor(camp => camp.Naziv).NotNull();
            RuleFor(camp => camp.Tip).NotNull();
            RuleFor(camp => camp.CijenaPoNoci).NotNull();
            RuleFor(camp => camp.Kontakt).NotNull();
            RuleFor(camp => camp.X).NotNull();
            RuleFor(camp => camp.Y).NotNull();
        }
    }
}
