using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Vodic.Validators
{
    public class VodicValidator: AbstractValidator<VodicAddVM>
    {
        public VodicValidator()
        {
            RuleFor(vodic => vodic.Ime).NotNull();
            RuleFor(vodic => vodic.Prezime).NotNull();
            RuleFor(vodic => vodic.Specijalizacija).NotNull();
            RuleFor(vodic => vodic.Iskustvo).NotNull();
            RuleFor(vodic => vodic.Lokacija).NotNull();
            RuleFor(vodic => vodic.Kontakt).NotNull();


        }
    }
}
