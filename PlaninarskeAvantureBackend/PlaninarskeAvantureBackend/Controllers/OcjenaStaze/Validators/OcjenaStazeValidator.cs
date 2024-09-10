using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.OcjenaStaze.Validators
{
    public class OcjenaStazeValidator:AbstractValidator<OcjenaStazeAddVM>
    {
        public OcjenaStazeValidator()
        {
            RuleFor(ocjena => ocjena.Komentar).NotNull();
            RuleFor(ocjena => ocjena.Ocjena).NotNull();
            RuleFor(ocjena => ocjena.Kvalitet).NotNull();
        }
    }
}
