using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.SigurnosniRizik.Validators
{
    public class SigurnosniRizikAddValidator: AbstractValidator<RizikAddVM>
    {
        public SigurnosniRizikAddValidator()
        {
            RuleFor(rizik => rizik.tipRizika).NotNull();
            RuleFor(rizik => rizik.opisRizika).NotNull();
            RuleFor(rizik => rizik.lokacijaRizika).NotNull();
            RuleFor(rizik => rizik.StazaId).NotNull();
        }
    }
}
