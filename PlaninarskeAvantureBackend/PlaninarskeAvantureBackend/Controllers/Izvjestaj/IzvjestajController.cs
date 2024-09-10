using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlaninarskeAvantureBackend.Controllers.Izvjestaj.IzvjestajValidator;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Izvjestaj
{
    [Route("api/[controller]")]
    [ApiController]
    public class IzvjestajController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public IzvjestajController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Dodaj([FromForm] IzvjestajAddVM _izvjestaj)
        {
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var dogadjaj = _dbContext.Dogadjaj.Where(x => x.Id == _izvjestaj.DogadjajId).FirstOrDefault();

            ApiResponse<Models.Data.SigurnosniIzvjestaj> response = new ApiResponse<Models.Data.SigurnosniIzvjestaj>();
            IzvjestajValidate validator = new IzvjestajValidate();
            ValidationResult result = validator.Validate(_izvjestaj);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = true;
                return BadRequest(response);
            }
            if (korisnik == null || dogadjaj == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani/dogadjaj nije pronadjen";
                return BadRequest(response);
            }
            if (dogadjaj.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Dogadjaj nije vas!";
                return BadRequest(response);
            }
            if (dogadjaj.VrijemeZavrsetka > DateTime.Now)
            {
                response.Success = false;
                response.Message = "Report mozete kreirati po zavrsetku dogadjaja!";
                return BadRequest(response);
            }
            var postojiLi = _dbContext.SigurnosniIzvjestaj.Any(x => x.DogadjajID == dogadjaj.Id);
            if (postojiLi)
            {
                response.Success = false;
                response.Message = "Mozete imati samo jedan izvjestaj po dogadzaju!";
                return BadRequest(response);
            }
            string ekstenzija = Path.GetExtension(_izvjestaj.SlikaIzvjestaj.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _izvjestaj.SlikaIzvjestaj.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));

            var noviReport = new Models.Data.SigurnosniIzvjestaj
            {
                x = _izvjestaj.x,
                y = _izvjestaj.y,
                Opis = _izvjestaj.Opis,
                Ostecenja = _izvjestaj.Ostecenja,
                Povreda = _izvjestaj.Povreda,
                Dogadjaj = dogadjaj,
                DogadjajID = dogadjaj.Id,
                Korisnik = korisnik,
                KorisnikId = korisnik.Id,
                DatumIzvjestaja = DateTime.Now,
                SlikaIzvjestaj = Config.SlikeURL + fileName
            };
            _dbContext.SigurnosniIzvjestaj.Add(noviReport);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Data = noviReport;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.SigurnosniIzvjestaj> response = new ApiResponse<Models.Data.SigurnosniIzvjestaj>();
            var izvjestaj = _dbContext.SigurnosniIzvjestaj.Where(x => x.Id == id).FirstOrDefault();
            if (izvjestaj == null)
            {
                response.Message = "Trazeni izvjestaj ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Success = true;
            response.Data = izvjestaj;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult IzvjestajUredi([FromForm]IzvjestajUrediVM _izvjestaj)
        {
            ApiResponse<Models.Data.SigurnosniIzvjestaj> response = new ApiResponse<Models.Data.SigurnosniIzvjestaj>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var izvjestaj = _dbContext.SigurnosniIzvjestaj.Where(x => x.Id == _izvjestaj.IzvjestajId).FirstOrDefault();
            if (izvjestaj == null)
            {
                response.Message = "Trazeni izvjestaj ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            else if (korisnik == null)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest(response);
            }
            else if (korisnik.Id != izvjestaj.KorisnikId)
            {
                response.Message = "Trazeni izvjestaj ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            izvjestaj.x = _izvjestaj.x;
            izvjestaj.y = _izvjestaj.y;
            izvjestaj.Opis = _izvjestaj.Opis;
            izvjestaj.Povreda = _izvjestaj.Povreda;
            izvjestaj.Ostecenja = _izvjestaj.Ostecenja;
            if (izvjestaj.SlikaIzvjestaj!= "")
            {
                var postojeca = izvjestaj.SlikaIzvjestaj;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var izvjestajSlika = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(izvjestajSlika);
            }
            string ekstenzija = Path.GetExtension(_izvjestaj.SlikaIzvjestaj.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _izvjestaj.SlikaIzvjestaj.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            izvjestaj.SlikaIzvjestaj = Config.SlikeURL + fileName;
            _dbContext.Update(izvjestaj);
            _dbContext.SaveChanges();
            return Ok(response);

        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.SigurnosniIzvjestaj> response = new ApiResponse<Models.Data.SigurnosniIzvjestaj>();
            var izvjestaj = _dbContext.SigurnosniIzvjestaj.Where(x => x.Id == id).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;

            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest();
            }else if (izvjestaj == null)
            {
                response.Success = false;
                response.Message = "Izvjestaj nije pronadjen!";
                return BadRequest();
            }
            else if (korisnik.Id != izvjestaj.KorisnikId) {
                response.Success = false;
                response.Message = "Izvjestaj nije vas!";
                return BadRequest();
            }
            if (izvjestaj.SlikaIzvjestaj != "")
            {
                var postojeca = izvjestaj.SlikaIzvjestaj;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var izvjestajSlika = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(izvjestajSlika);
            }
            _dbContext.SigurnosniIzvjestaj.Remove(izvjestaj);
            _dbContext.SaveChanges();
            response.Message = "Izvjestaj obrisan!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet]
        public IActionResult GetIzvjestajByEvent(int eventId)
        {
            ApiResponse<Models.Data.SigurnosniIzvjestaj> response = new ApiResponse<Models.Data.SigurnosniIzvjestaj>();
            var izvjestaj = _dbContext.SigurnosniIzvjestaj.Where(x => x.DogadjajID == eventId).FirstOrDefault();
            if (izvjestaj == null)
            {
                response.Success = false;
                response.Message = "Izvjestaj nije kreiran za trazeni dogadzaj!";
                return BadRequest(response);
            }
            response.Data = izvjestaj;
            response.Success = true;
            return Ok(response);
        }
    }
}
