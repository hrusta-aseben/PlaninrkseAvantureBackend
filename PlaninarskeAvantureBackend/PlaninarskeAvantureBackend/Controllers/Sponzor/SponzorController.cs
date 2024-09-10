using Azure;
using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaninarskeAvantureBackend.Controllers.Sponzor.SponzorValidators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Sponzor
{
    [Route("api/[controller]")]
    [ApiController]
    public class SponzorController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public SponzorController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult SponzorAdd([FromForm]SponzorAddVM sponzor)
        {
            ApiResponse<Models.Data.Sponzor> response = new ApiResponse<Models.Data.Sponzor>();
            SponzorAddValidator validator = new SponzorAddValidator();
            ValidationResult result=validator.Validate(sponzor);
            if (!result.IsValid)
            {
                response.Message=result.ToString();
                response.Success = false;
                return BadRequest();
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (korisnik == null)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest();
            }
            var dogadjaj = _dbContext.Dogadjaj.Include(x=>x.Korisnik).Where(x => x.Id == sponzor.DogadjajId).FirstOrDefault();
            var kreator = dogadjaj.Korisnik;
            if (kreator!= korisnik)
            {
                response.Message = "Sponzore mozete dodati samo na vase dogadjaje!";
                response.Success = false;
                return BadRequest();
            }
            string ekstenzija = Path.GetExtension(sponzor.SponzorSlika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            sponzor.SponzorSlika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var noviSponzor = new Models.Data.Sponzor
            {
                Naziv = sponzor.Naziv,
                Tip = sponzor.Tip,
                Kontakt = sponzor.Kontakt,
                Dogadjaj = dogadjaj,
                DogadjajId = dogadjaj.Id,
                Korisnik=korisnik,
                KorisnikId=korisnik.Id,
                SponzorSlika=Config.SlikeURL+fileName
            };
            _dbContext.Sponzor.Add(noviSponzor);
            _dbContext.SaveChanges();
            response.Data = noviSponzor;
            response.Success = true;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult SponzorEdit([FromForm] SponzorEditVM _sponzor)
        {
            ApiResponse<Models.Data.Sponzor> response = new ApiResponse<Models.Data.Sponzor>();
            var sponzor =_dbContext.Sponzor.Where(x=>x.Id==_sponzor.SponzorId).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if(sponzor==null)
            {
                response.Message = "Sponzor nije pronadjen!";
                response.Success = false;
                return BadRequest(response);
            }
            if (korisnik == null)
            {
                response.Message = "Sponzor nije pronadjen!";
                response.Success = false;
                return BadRequest(response);
            }
            if (korisnik.Id != sponzor.KorisnikId)
            {
                response.Message = "Trazeni sponzor ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            sponzor.Naziv = _sponzor.Naziv;
            sponzor.Kontakt= _sponzor.Kontakt;
            sponzor.Tip = _sponzor.Tip;
            if (sponzor.SponzorSlika != "")
            {
                var postojeca = sponzor.SponzorSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaSponzora = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaSponzora);
            }
            string ekstenzija = Path.GetExtension(_sponzor.SponzorSlika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _sponzor.SponzorSlika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            sponzor.SponzorSlika = Config.SlikeURL + fileName;
            _dbContext.Update(sponzor);
            _dbContext.SaveChanges();
            response.Data = sponzor;
            response.Success = true;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.Sponzor> response = new ApiResponse<Models.Data.Sponzor>();
            var sponzor = _dbContext.Sponzor.Where(x => x.Id == id).FirstOrDefault();
            if (sponzor == null)
            {
                response.Success = false;
                response.Message = "Sponzorstvo nije pronadjeno!";
                return BadRequest(response);
            }
            response.Data = sponzor;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.Sponzor> response = new ApiResponse<Models.Data.Sponzor>();
            var sponzor = _dbContext.Sponzor.Where(x => x.Id == id).FirstOrDefault();
            if (sponzor == null)
            {
                response.Success = false;
                response.Message = "Sponzorstvo nije pronadjeno!";
                return BadRequest(response);
            }
            if (sponzor.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Success = false;
                response.Message = "Drustvo nije vase!";
                return BadRequest(response);
            }
            if (sponzor.SponzorSlika != "")
            {
                var postojeca = sponzor.SponzorSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaSponzora = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaSponzora);
            }
            _dbContext.Sponzor.Remove(sponzor);
            _dbContext.SaveChanges();
            response.Message = "Sponzorstvo obrisano!";
            response.Success = true;
            return Ok(response);
        }
    }
}
