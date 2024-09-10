using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;
using PlaninarskeAvantureBackend.Controllers.Izvjestaj.IzvjestajValidator;
using PlaninarskeAvantureBackend.Controllers.Putopis.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.Models.Data;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Putopis
{
    [Route("api/[controller]")]
    [ApiController]
    public class PutopisController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public PutopisController(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult DodajPutopis([FromBody] PutopisAddVM putopis)
        {
            var korisnik=_authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.Putopis> response = new ApiResponse<Models.Data.Putopis>();
            PutopisCreateValidator validator = new PutopisCreateValidator();
            ValidationResult result=validator.Validate(putopis);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            if(korisnik==null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest(response);
            }

            var noviPutopis = new Models.Data.Putopis
            {
                Opis = putopis.Opis,
                Naziv = putopis.Naziv,
                Korisnik = korisnik,
                DatumObjavljivanja = DateTime.Now,
            };
            _dbContext.Putopis.Add(noviPutopis);
            _dbContext.SaveChanges();
            foreach(var p in putopis.Staza)
            {
                var ptpstz = new PutopisStaze
                {
                    StazaId = p,
                    PutopisId = noviPutopis.Id
                };
                _dbContext.PutopisStaze.Add( ptpstz );
                _dbContext.SaveChanges();
            }
            response.Success = true;
            response.Data = noviPutopis;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<PutopisGetStaze> response = new ApiResponse<PutopisGetStaze>();

            var lista = _dbContext.PutopisStaze.Where(x => x.PutopisId == id).Select(x => x.Staza).ToList();
            var putopis = _dbContext.Putopis.       
                Where(x => x.Id == id).FirstOrDefault();

            var aa = new PutopisGetStaze
            {
                Putopis = putopis,
                Staza = lista
            };
            response.Data = aa;
            response.Success = true;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult Edit(PutopisEditVM _putopis) {
            var korisnik = _authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.Putopis> response = new ApiResponse<Models.Data.Putopis>();

            var putopis = _dbContext.Putopis.Where(x => x.Id == _putopis.Id).FirstOrDefault();
            if (putopis.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Putopis nije vas!";
                return BadRequest();
            }
            var id=putopis.Id;
            _dbContext.Remove(putopis);
            _dbContext.SaveChanges();
            var novi = new Models.Data.Putopis
            {
                Naziv = _putopis.Naziv,
                Opis = _putopis.Opis,
                Korisnik = korisnik,
                DatumObjavljivanja = DateTime.Now
            };
            _dbContext.Putopis.Add(novi);
            _dbContext.SaveChanges();
            var lista=_dbContext.PutopisStaze.Where(x=>x.PutopisId== id).ToList();
            _dbContext.RemoveRange(lista);
            _dbContext.SaveChanges();
            foreach (var p in _putopis.Staza)
            {
                var ptpstz = new PutopisStaze
                {
                    StazaId = p,
                    PutopisId = novi.Id
                };
                _dbContext.PutopisStaze.Add(ptpstz);
                _dbContext.SaveChanges();
            }

            response.Success=true;
            response.Data = novi;

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            var korisnik = _authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.Putopis> response = new ApiResponse<Models.Data.Putopis>();
            var putopis = _dbContext.Putopis.Where(x => x.Id == id).FirstOrDefault();
            if (putopis.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Putopis nije vas!";
                return BadRequest();
            }
            var lista = _dbContext.PutopisStaze.Where(x => x.PutopisId == id).ToList();
            if (lista.Count > 0)
            {
                _dbContext.RemoveRange(lista);
                _dbContext.SaveChanges();
            }
            _dbContext.Putopis.Remove(putopis);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = "Uspjesno obrisan!";

            return Ok(response);
        }
    }
}
