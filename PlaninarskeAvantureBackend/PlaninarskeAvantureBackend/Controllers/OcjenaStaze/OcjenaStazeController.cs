using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaninarskeAvantureBackend.Controllers.OcjenaStaze.Validators;
using PlaninarskeAvantureBackend.Controllers.Putopis.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.OcjenaStaze
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OcjenaStazeController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public OcjenaStazeController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult DodajOcjenu([FromBody] OcjenaStazeAddVM ocjena)
        {
            ApiResponse<Models.Data.OcjenaStaze> response = new ApiResponse<Models.Data.OcjenaStaze>();
            OcjenaStazeValidator validator = new OcjenaStazeValidator();
            ValidationResult result = validator.Validate(ocjena);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = true;
                return BadRequest(response);
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest(response);
            }
            var postojiLi = _dbContext.OcjenaStaze.Any(x => x.KorisnikId == korisnik.Id && x.StazaId == ocjena.StazaId);
            if (postojiLi)
            {
                response.Success = false;
                response.Message = "Staza je vec ocjenjena!";
                return BadRequest(response);
            }
            var novaOcjena = new Models.Data.OcjenaStaze
            {
                Komentar = ocjena.Komentar,
                Ocjena = ocjena.Ocjena,
                Kvalitet = ocjena.Kvalitet,
                StazaId= ocjena.StazaId,
                Korisnik=korisnik,
                KorisnikId=korisnik.Id
            };
            _dbContext.OcjenaStaze.Add(novaOcjena);
            _dbContext.SaveChanges();
            response.Data = novaOcjena;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.OcjenaStaze> response = new ApiResponse<Models.Data.OcjenaStaze>();
            var ocjena=_dbContext.OcjenaStaze.Where(x=>x.Id== id).FirstOrDefault();
            if (ocjena == null)
            {
                response.Success = false;
                response.Message = "Trazena ocjena nije pronadjena!";
                return BadRequest();
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if(korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest();
            }
            if (korisnik.Id != ocjena.KorisnikId)
            {
                response.Success = false;
                response.Message = "Ocjena koju pokusavate obrisati nije vasa!";
                return BadRequest();
            }
            _dbContext.Remove(ocjena);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = "Uspjesno obrisana ocjena!";
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult OcjeneZaStazu(int id)
        {
            ApiResponse<List<Models.Data.OcjenaStaze>> response = new ApiResponse<List<Models.Data.OcjenaStaze>>();
            var ocjene = _dbContext.OcjenaStaze.Include(x => x.Korisnik).Where(x => x.StazaId == id).ToList();
            if (ocjene == null)
            {
                response.Success = false;
                response.Message = "Nije pronadjena ocjena za ovu stazu!";
                return BadRequest();
            }

            return Ok(ocjene);
        }
        [HttpPut]
        public IActionResult UrediOcjenu([FromBody]OcjenaUrediVM _ocjena)
        {
            ApiResponse<Models.Data.OcjenaStaze> response = new ApiResponse<Models.Data.OcjenaStaze>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var ocjena = _dbContext.OcjenaStaze.Where(x => x.Id == _ocjena.Id).FirstOrDefault();
            if(korisnik==null)
            {
                response.Success = false;
                response.Message = "Niste prijavljeni!";
                return BadRequest(response);
            }
            if (ocjena == null) {
                response.Success = false;
                response.Message = "Ocjena nije pronadjena!";
                return BadRequest(response);
            }

            if (korisnik.Id != ocjena.KorisnikId)
            {
                response.Success = false;
                response.Message = "Ocjena nije vasa!";
                return BadRequest(response);
            }
            ocjena.Komentar = _ocjena.Komentar;
            ocjena.Ocjena = _ocjena.Ocjena;
            ocjena.Kvalitet = _ocjena.Kvalitet;
            _dbContext.Update(ocjena);
            _dbContext.SaveChanges();
            response.Data = ocjena;
            response.Success = true;
            return Ok(response);
        }
    }
}
