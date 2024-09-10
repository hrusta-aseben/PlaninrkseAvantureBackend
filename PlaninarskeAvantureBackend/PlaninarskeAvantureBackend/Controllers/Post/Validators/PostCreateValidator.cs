using FIT_Api_Example.ViewModels;
using FluentValidation;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Post.Validators
{
        public class PostCreateValidator : AbstractValidator<PostAddVM>
        {
            public PostCreateValidator()
            {
                RuleFor(korisnik => korisnik.Tekst).NotNull();
                RuleFor(korisnik => korisnik.Vrijeme).NotNull();
                RuleFor(korisnik => korisnik.Tezina).NotNull();

            }

        }
}
