using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.Vodic.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Vodic
{
    [Route("api/vodic/[action]")]
    [ApiController]
    public class VodicController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public VodicController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Dodaj([FromBody] VodicAddVM _vodic)
        {
            var korisnik = _authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.Vodic> response = new ApiResponse<Models.Data.Vodic>();
            VodicValidator validator = new VodicValidator();
            ValidationResult result=validator.Validate(_vodic);
            var daLiPostoji = _dbContext.Vodic.Any(x => x.KorisnikId == korisnik.Id);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            else if (korisnik == null)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest(response);
            }
            else if(daLiPostoji)
            {
                response.Message = "Vec imate svog vodica!";
                response.Success = false;
                return BadRequest(response);
            }
            var noviVodic = new Models.Data.Vodic
            {
                Ime = _vodic.Ime,
                Prezime = _vodic.Prezime,
                Iskustvo = _vodic.Iskustvo,
                Specijalizacija = _vodic.Specijalizacija,
                Lokacija = _vodic.Lokacija,
                Korisnik = korisnik,
                Kontakt=_vodic.Kontakt
            };
            response.Data = noviVodic;
            response.Success = true;
            _dbContext.Add(noviVodic);
            _dbContext.SaveChanges();
            return Ok(response);
        }
        [HttpPut]
        public IActionResult Uredi([FromBody] VodicUrediVM _vodic)
        {
            ApiResponse<Models.Data.Vodic> response = new ApiResponse<Models.Data.Vodic>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var vodic = _dbContext.Vodic.Where(x => x.Id == _vodic.VodicID).FirstOrDefault();
            if (vodic == null)
            {
                response.Message = "Trazeni vodic ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            else if (korisnik == null)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest(response);
            }
            else if(korisnik.Id != vodic.KorisnikId)
            {
                response.Message = "Trazeni vodic ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            vodic.Ime = _vodic.Ime;
            vodic.Prezime = _vodic.Prezime;
            vodic.Lokacija= _vodic.Lokacija;
            vodic.Specijalizacija=_vodic.Specijalizacija;
            vodic.Iskustvo=_vodic.Iskustvo;
            vodic.Kontakt = _vodic.Kontakt;
            _dbContext.Update(vodic);
            _dbContext.SaveChanges();
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.Vodic> response = new ApiResponse<Models.Data.Vodic>();
            var vodic = _dbContext.Vodic.Where(x => x.Id == id).FirstOrDefault();
            if(vodic==null)
            {
                response.Message = "Trazeni vodic ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Success= true;
            response.Data= vodic;
            return Ok(response);
        }
        [HttpDelete]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.Vodic> response = new ApiResponse<Models.Data.Vodic>();
            var vodic = _dbContext.Vodic.Where(x => x.Id == id).FirstOrDefault();
            if (vodic == null)
            {
                response.Message = "Trazeni vodic ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            else if (vodic.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Message = "Vodic nije vas!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Success= true;
            _dbContext.Remove(vodic);
            _dbContext.SaveChanges();
            return Ok(response);
        }
        [HttpGet("{lokacija}")]
        public IActionResult GetVodicByLokacija(string lokacija)
        {
            ApiResponse<List<Models.Data.Vodic>> response = new ApiResponse<List<Models.Data.Vodic>>();
            var vodic = _dbContext.Vodic.Where(x => lokacija.Contains(x.Lokacija)).ToList();
            if (vodic.IsNullOrEmpty())
            {
                response.Message = "Nema vodica za trazenu lokaciju!";
                return Ok(response);
            }
            response.Data = vodic;
            return Ok(response);
        }

    }
}
