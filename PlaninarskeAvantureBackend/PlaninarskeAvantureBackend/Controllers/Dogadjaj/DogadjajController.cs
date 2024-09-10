using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.Dogadjaj.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Dogadjaj
{
    [Route("api/dogadjaj/[action]")]
    [ApiController]
    public class DogadjajController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authService;
        public DogadjajController(ApplicationDbContext applicationDbContext, AuthService authService)
        {
            _applicationDbContext = applicationDbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Kreiranje([FromForm] DogadjajAddVM dogadjaj)
        {
            ApiResponse<Models.Data.Dogadjaj> response = new ApiResponse<Models.Data.Dogadjaj>();
            DogadjajAddValidator validator = new DogadjajAddValidator();
            ValidationResult result = validator.Validate(dogadjaj);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            if (!_authService.Logiran())
            {
                response.Message = "Niste ulogovani";
                response.Success = false;
                return BadRequest(response);
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            string ekstenzija = Path.GetExtension(dogadjaj.SlikaDogadjaja.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            dogadjaj.SlikaDogadjaja.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var noviDogadja = new Models.Data.Dogadjaj
            {
                Naziv = dogadjaj.Naziv,
                VrijemePocetka = dogadjaj.VrijemePocetka,
                VrijemeZavrsetka = dogadjaj.VrijemeZavrsetka,
                Lokacija = dogadjaj.Lokacija,
                Opis = dogadjaj.Opis,
                VrijemeKreiranja = DateTime.Now,
                BrojPlaninara = dogadjaj.BrojPlaninara,
                KorisnikId = korisnik.Id
            };
            noviDogadja.SlikaDogadjaja = Config.SlikeURL + fileName;
            _applicationDbContext.Dogadjaj.Add(noviDogadja);
            _applicationDbContext.SaveChanges();
            response.Success = true;
            response.Data = noviDogadja;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult Uredi([FromForm] DogadjajUrediVM _dogadjaj)
        {
            ApiResponse<Models.Data.Dogadjaj> apiResponse = new ApiResponse<Models.Data.Dogadjaj>();
            var dogadjaj = _applicationDbContext.Dogadjaj.Where(x => x.Id == _dogadjaj.DogadjajID).FirstOrDefault();
            if (_authService.GetInfo().korisnickiNalog == null)
            {
                apiResponse.Message = "Niste ulogovani";
                apiResponse.Success = false;
                return BadRequest(apiResponse);
            }
            if (dogadjaj == null)
            {
                apiResponse.Message = "Dogadjaj nije pronadjen";
                apiResponse.Success = false;
                return BadRequest(apiResponse);
            }
            if (dogadjaj.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                apiResponse.Message = "Dogadjaj ne pripada vama!";
                apiResponse.Success = false;
                return BadRequest(apiResponse);
            }

            dogadjaj.Naziv = _dogadjaj.Naziv;
            dogadjaj.VrijemePocetka = _dogadjaj.VrijemePocetka;
            dogadjaj.VrijemeZavrsetka = _dogadjaj.VrijemeZavrsetka;
            dogadjaj.Lokacija = _dogadjaj.Lokacija;
            dogadjaj.xKordinata = _dogadjaj.xKordinata;
            dogadjaj.yKordinata = _dogadjaj.yKordinata;
            dogadjaj.Opis = _dogadjaj.Opis;
            dogadjaj.BrojPlaninara = _dogadjaj.BrojPlaninara;
            if (dogadjaj.SlikaDogadjaja != "")
            {
                var postojeca = dogadjaj.SlikaDogadjaja;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaDogadjaj = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaDogadjaj);
            }
            string ekstenzija = Path.GetExtension(_dogadjaj.Slika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _dogadjaj.Slika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            dogadjaj.SlikaDogadjaja = Config.SlikeURL + fileName;
            apiResponse.Data = dogadjaj;
            apiResponse.Message = "Uspjesno uredjen dogadjaj!";
            _applicationDbContext.Dogadjaj.Update(dogadjaj);
            _applicationDbContext.SaveChanges();
            return new JsonResult(new { poruka = "Uredjen dogadjaj" });
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.Dogadjaj> response = new ApiResponse<Models.Data.Dogadjaj>();
            var dogadjaj = _applicationDbContext.Dogadjaj.Include(x => x.Korisnik).Where(x => x.Id == id).FirstOrDefault();
            if (dogadjaj == null)
            {
                response.Success = false;
                response.Message = "Dogadjaj nije pronadjen!";
                return BadRequest(response);
            }
            response.Data = dogadjaj;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.Dogadjaj> response = new ApiResponse<Models.Data.Dogadjaj>();
            var dogadjaj = _applicationDbContext.Dogadjaj.Where(x => x.Id == id).FirstOrDefault();
            if (dogadjaj == null)
            {
                response.Success = false;
                response.Message = "Dogadjaj nije pronadjen!";
                return BadRequest(response);
            }
            if (dogadjaj.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Success = false;
                response.Message = "Dogadjaj nije vas!";
                return BadRequest(response);
            }
            if (dogadjaj.SlikaDogadjaja != "")
            {
                var postojeca = dogadjaj.SlikaDogadjaja;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaDogadjaj = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaDogadjaj);
            }
            _applicationDbContext.Dogadjaj.Remove(dogadjaj);
            _applicationDbContext.SaveChanges();
            response.Message = "Dogadjaj obrisan!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet]
        public List<Models.Data.Dogadjaj> GetAll()
        {
            ApiResponse<Models.Data.Dogadjaj> response = new ApiResponse<Models.Data.Dogadjaj>();
            var dogadjaji = _applicationDbContext.Dogadjaj.OrderBy(x => x.VrijemeKreiranja).Take(100).ToList();

            return dogadjaji;
        }

        [HttpPost("{id}")]
        public IActionResult JoinEvent(int id)
        {
            ApiResponse<Models.Data.Dogadjaj> response = new ApiResponse<Models.Data.Dogadjaj>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var dogadjaj = _applicationDbContext.Dogadjaj.Where(x => x.Id == id).FirstOrDefault();
            if (dogadjaj == null)
            {
                response.Success = false;
                response.Message = "Trazeni dogadjaj nije pronadjen!";
                return BadRequest(response);
            }
            var ideLi = _applicationDbContext.DogadjajKorisnici.Any(x => x.KorisnikId == korisnik.Id && dogadjaj.Id == x.DogadjajID);
            if (ideLi)
            {
                var zapis = _applicationDbContext.DogadjajKorisnici.Where(x => x.KorisnikId == korisnik.Id && dogadjaj.Id == x.DogadjajID).FirstOrDefault();
                _applicationDbContext.DogadjajKorisnici.Remove(zapis);
                _applicationDbContext.SaveChanges();
                response.Success = true;
                response.Message = "Odjavljeni ste sa dogadjaja!";
                return Ok(response);
            }
            var broj = _applicationDbContext.DogadjajKorisnici.Where(x => x.DogadjajID == dogadjaj.Id).Count();
            if (broj >= dogadjaj.BrojPlaninara)
            {
                response.Success = false;
                response.Message = "Broj mjesta popunjen!";
                return BadRequest(response);
            }
            var noviGoer = new Models.Data.DogadjajKorisnici
            {
                Dogadjaj = dogadjaj,
                DogadjajID = dogadjaj.Id,
                Korisnik = korisnik,
                KorisnikId = korisnik.Id
            };
            _applicationDbContext.DogadjajKorisnici.Add(noviGoer);
            _applicationDbContext.SaveChanges();
            response.Message = "Uspjesna prijava na dogadjaj!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetSponzoriByDogadjaj(int id)
        {
            ApiResponse<List<Models.Data.Sponzor>> response = new ApiResponse<List<Models.Data.Sponzor>>();
            var sponzori = _applicationDbContext.Sponzor.Where(x => x.DogadjajId == id).ToList();
            if (sponzori.IsNullOrEmpty())
            {
                response.Message = "Trazeni dogadjaj nema sponzora!";
                return Ok(response);
            }
            response.Data=sponzori;
            return Ok(response);
        }

    }
}
