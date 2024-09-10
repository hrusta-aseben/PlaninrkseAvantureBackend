using Azure;
using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.SigurnosniRizik.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.SigurnosniRizik
{
    [Route("api/[controller]")]
    [ApiController]
    public class SigurnosniRizikController : ControllerBase
    {
        readonly private AuthService _authService;
        readonly private ApplicationDbContext    _dbContext;
        public SigurnosniRizikController(AuthService authService, ApplicationDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }
        [HttpPost]
        public IActionResult DodajRizik([FromForm] RizikAddVM _rizik)
        {
            ApiResponse<Models.Data.SigurnosniRizik> response = new ApiResponse<Models.Data.SigurnosniRizik>();
            SigurnosniRizikAddValidator validator = new SigurnosniRizikAddValidator();
            ValidationResult result = validator.Validate(_rizik);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest();
            }
            if (!_authService.Logiran())
            {
                response.Message = "Niste ulogovani";
                response.Success = false;
                return BadRequest(response);
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var staza = _dbContext.Staza.Where(x => x.Id == _rizik.StazaId).FirstOrDefault();
            if (staza == null)
            {
                response.Message = "Staza nije pronadjena!";
                response.Success = false;
                return BadRequest(response);
            }
            if (staza.KorisnikId != korisnik.Id)
            {
                response.Message = "Rizik mozete dodatai samo na vase staze!";
                response.Success = false;
                return BadRequest(response);
            }
            string ekstenzija = Path.GetExtension(_rizik.RizikSlika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _rizik.RizikSlika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var rizik = new Models.Data.SigurnosniRizik
            {
                tipRizika = _rizik.tipRizika,
                opisRizika = _rizik.opisRizika,
                lokacijaRizika = _rizik.lokacijaRizika,
                Staza = staza,
                StazaId = staza.Id,
                RizikSlika = Config.SlikeURL + fileName
            };
            _dbContext.SigurnosniRizik.Add(rizik);
            _dbContext.SaveChanges();
            response.Data = rizik;
            response.Success = true;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult UrediRizik([FromForm] SigurnosniRizikUrediVM _rizik)
        {
            ApiResponse<Models.Data.SigurnosniRizik> response = new ApiResponse<Models.Data.SigurnosniRizik>();
            var rizik = _dbContext.SigurnosniRizik.Include(x=>x.Staza).Where(x => x.Id == _rizik.rizikId).FirstOrDefault();
            if (rizik == null)
            {
                response.Message = "Trazeni rizik ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            var staza = rizik.Staza;
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (staza.KorisnikId != korisnik.Id)
            {
                response.Message = "Trazeni rizik ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            rizik.tipRizika = _rizik.tipRizika;
            rizik.opisRizika=_rizik.opisRizika;
            rizik.lokacijaRizika = _rizik.lokacijaRizika;
            if (rizik.RizikSlika!= "")
            {
                var postojeca = rizik.RizikSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var rizikSlika = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(rizikSlika);
            }
            string ekstenzija = Path.GetExtension(_rizik.RizikSlika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _rizik.RizikSlika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            rizik.RizikSlika = Config.SlikeURL + fileName;
            _dbContext.Update( rizik );
            _dbContext.SaveChanges();
            response.Data = rizik;
            response.Success = true;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.SigurnosniRizik> response = new ApiResponse<Models.Data.SigurnosniRizik>();
            var rizik = _dbContext.SigurnosniRizik.Where(x => x.Id == id).FirstOrDefault();
            if (rizik == null)
            {
                response.Success = false;
                response.Message = "Trazeni rizik nije pronadjen!";
                return BadRequest(response);
            }
            response.Data = rizik;
            response.Success = true;
            return Ok(response.Data);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteById(int id)
        {
            ApiResponse<Models.Data.SigurnosniRizik> response = new ApiResponse<Models.Data.SigurnosniRizik>();
            var rizik = _dbContext.SigurnosniRizik.Where(x => x.Id == id).FirstOrDefault();
            if (rizik == null)
            {
                response.Success = false;
                response.Message = "Rizik nije pronadjen!";
                return BadRequest(response);
            }
            if(rizik.Staza.Korisnik != _authService.GetInfo().korisnickiNalog)
            {
                response.Success = false;
                response.Message = "Rizik nije vas!";
                return BadRequest(response);
            }
            if (rizik.RizikSlika != "")
            {
                var postojeca = rizik.RizikSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaRizika = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaRizika);
            }
            _dbContext.Remove(rizik);
            _dbContext.SaveChanges();
            response.Message = "Rizik obrisan!";
            response.Success = true;
            return Ok(response.Message);
        }
        [HttpGet("stazaId")]
        public IActionResult GetRizikByStaza(int stazaId)
        {
            ApiResponse<List<Models.Data.SigurnosniRizik>> response = new ApiResponse<List<Models.Data.SigurnosniRizik>>();
            var staza = _dbContext.Staza.Where(x => x.Id == stazaId).FirstOrDefault();
            if (staza == null)
            {
                response.Success = false;
                response.Message = "Nije pronadjena staza!";
                return BadRequest(response);
            }
            var rizici = _dbContext.SigurnosniRizik.Where(x => x.StazaId == staza.Id).ToList();
            if (rizici.IsNullOrEmpty())
            {
                response.Success = false;
                response.Message = "Nisu pronadjeni rizici za stazu!";
                return BadRequest(response);
            }
            response.Data = rizici;
            return Ok(response);
        }
    }
}
