using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.Oprema.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Oprema
{
    [Route("api/oprema/[action]")]
    [ApiController]
    public class OpremaController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public OpremaController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Dodaj([FromForm] OpremaAddVM _oprema)
        {
            ApiResponse<Models.Data.Oprema> response = new ApiResponse<Models.Data.Oprema>();
            OpremaValidator validator = new OpremaValidator();
            ValidationResult result = validator.Validate(_oprema);
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            if (!_authService.GetInfo().isLogiran)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest(response);
            }
            string ekstenzija = Path.GetExtension(_oprema.SlikaOpreme.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _oprema.SlikaOpreme.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var novaOprema = new Models.Data.Oprema
            {
                Naziv = _oprema.Naziv,
                Opis = _oprema.Opis,
                Kategorija = _oprema.Kategorija,
                Proizvodjac = _oprema.Proizvodjac,
                Recenzija = _oprema.Recenzija,
                Korisnik=korisnik,
                KorisnikId=korisnik.Id,
                SlikaOpreme=Config.SlikeURL+fileName
            };
            _dbContext.Oprema.Add(novaOprema);
            _dbContext.SaveChanges();
            response.Data = novaOprema;
            response.Success = true;
            return Ok(response);
        }
 
        [HttpGet("{id}")]
        public IActionResult GetOpremyById(int id)
        {
            ApiResponse<Models.Data.Oprema> response = new ApiResponse<Models.Data.Oprema>();
            var oprema = _dbContext.Oprema.Where(x => x.Id == id).FirstOrDefault();
            if (oprema == null)
            {
                response.Success = false;
                response.Message = "Trazena oprema nije pronadjena!";
                return BadRequest(response);
            }
            response.Data = oprema;
            response.Success = true;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult Uredi([FromForm] OpremaUredi _oprema) {
            ApiResponse<Models.Data.Oprema> response = new ApiResponse<Models.Data.Oprema>();
            var oprema=_dbContext.Oprema.Where(x=>x.Id==_oprema.Id).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (oprema == null)
            {
                response.Success = false;
                response.Message = "Nije pronadjena oprema!";
                return BadRequest(response);
            }
            else if (korisnik.Id != oprema.KorisnikId)
            {
                response.Success = false;
                response.Message="Nije vasa oprema!";
                return BadRequest(response);
            }
            oprema.Naziv = _oprema.Naziv;
            oprema.Opis = _oprema.Opis;
            oprema.Kategorija = _oprema.Kategorija;
            oprema.Proizvodjac = _oprema.Proizvodjac;
            oprema.Recenzija = _oprema.Recenzija;
            if (oprema.SlikaOpreme != "")
            {
                var postojeca = oprema.SlikaOpreme;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaOpreme = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaOpreme);
            }
            string ekstenzija = Path.GetExtension(_oprema.SlikaOpreme.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _oprema.SlikaOpreme.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            oprema.SlikaOpreme = Config.SlikeURL + fileName;
            _dbContext.Update(oprema);
            _dbContext.SaveChanges();
            response.Data= oprema;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete]
        public IActionResult Obrisi(int id)
        {

            ApiResponse<Models.Data.Oprema> response = new ApiResponse<Models.Data.Oprema>();
            var oprema = _dbContext.Oprema.Where(x => x.Id == id).FirstOrDefault();
            if (oprema == null)
            {
                response.Success = false;
                response.Message = "Oprema nije pronadjena!";
                return BadRequest(response);
            }
            if (oprema.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Success = false;
                response.Message = "Oprema nije vasa!";
                return BadRequest(response);
            }
            if (oprema.SlikaOpreme != "")
            {
                var postojeca = oprema.SlikaOpreme;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaOpreme = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaOpreme);
            }
            _dbContext.Oprema.Remove(oprema);
            _dbContext.SaveChanges();
            response.Message = "Oprema obrisana!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet("{korisnikId}")]
        public IActionResult GetOpremaByKorisnik(int korisnikId)
        {
            ApiResponse<List<Models.Data.Oprema>> response = new ApiResponse<List<Models.Data.Oprema>>();
            var oprema = _dbContext.Oprema.Where(x => x.KorisnikId == korisnikId).ToList();
            if (oprema.IsNullOrEmpty())
            {
                response.Message = "Trazeni korisnik nema opremu!";
                return Ok(response);
            }
            response.Data = oprema;
            return Ok(response);
        }
    }
}
