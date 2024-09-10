using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Putopis.Validators
{
    public class PutopisCreateValidator:AbstractValidator<PutopisAddVM>
    {
        public PutopisCreateValidator()
        {
            RuleFor(putopis=>putopis.Opis).NotEmpty();
            RuleFor(putopis => putopis.Naziv).NotEmpty();
            RuleFor(putopis=>putopis.Staza).NotEmpty();
        }
    }
}
