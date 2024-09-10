using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.Drustvo.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.Models.Data;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Drustvo
{
    [Route("api/group/[action]")]
    [ApiController]
    public class DrustvoController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public DrustvoController(ApplicationDbContext dbContext, AuthService authService)
        {
            _authService= authService;
            _dbContext = dbContext;
        }
        [HttpPost]
        public IActionResult KreirajDrustvo([FromForm] KreirajDrustvoVM drustvo)
        {
            ApiResponse<Models.Data.PlaninarskoDrustvo> response = new ApiResponse<Models.Data.PlaninarskoDrustvo>();
            DrustvoCreateValidator validator = new DrustvoCreateValidator();
            ValidationResult result = validator.Validate(drustvo);
            if(!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (korisnik == null)
            {
                response.Message = "Morate biti ulogovani!";
                response.Success = false;
                return BadRequest(response);
            }
            string ekstenzija = Path.GetExtension(drustvo.SlikaDrustva.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            drustvo.SlikaDrustva.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));

            var _drustvo = new Models.Data.PlaninarskoDrustvo
            {
                Naziv = drustvo.Naziv,
                Adresa = drustvo.Adresa,
                Kontakt = drustvo.Kontakt,
                Opis = drustvo.Opis,
                KorisnikAdmin = korisnik,
                KorisnikId = korisnik.Id,
                DrustvoSlika = Config.SlikeURL + fileName
            };
            var prviCLan = new DrustvoKorisnici
            {
                PlaninarskoDrustvo = _drustvo,
                PlaninarskoDrustvoID = _drustvo.Id,
                Korisnik = korisnik,
                KorisnikId = korisnik.Id
            };
            _dbContext.DrustvoKorisnici.Add(prviCLan);
            _dbContext.PlaninarskoDrustvo.Add(_drustvo);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Data = _drustvo;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult UrediDrustvo([FromForm] DrustvoUrediVM _drustvo)
        {
            var drustvo=_dbContext.PlaninarskoDrustvo.Where(x=>x.Id==_drustvo.drustvoId).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.PlaninarskoDrustvo> response = new ApiResponse<Models.Data.PlaninarskoDrustvo>();
            if (drustvo == null)
            {
                response.Success = false;
                response.Message = "Drustvo nije pronadjeno";
                return BadRequest(response);
            }
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest(response);
            }
            if (korisnik.Id == drustvo.KorisnikId)
            {
                drustvo.Naziv = _drustvo.Naziv;
                drustvo.Adresa = _drustvo.Adresa;
                drustvo.Kontakt = _drustvo.Kontakt;
                drustvo.Opis = _drustvo.Opis;
                if (drustvo.DrustvoSlika != "")
                {
                    var postojeca = drustvo.DrustvoSlika;
                    string[] parts = postojeca.Split('/');
                    string imeFajla = parts[^1];
                    var slikaDrustva = Config.SlikeFolder + imeFajla;

                    System.GC.Collect();
                    System.GC.WaitForPendingFinalizers();
                    System.IO.File.Delete(slikaDrustva);
                }
                string ekstenzija = Path.GetExtension(_drustvo.SlikaDrustva.FileName);
                var fileName = $"{Guid.NewGuid()}{ekstenzija}";
                _drustvo.SlikaDrustva.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
                drustvo.DrustvoSlika = Config.SlikeURL + fileName;
            }
            else
            {
                response.Success = false;
                response.Message = "Nije vase drustvo!";
                return BadRequest(response);
            }

            response.Data = drustvo;
            response.Success = true;
            _dbContext.PlaninarskoDrustvo.Update(drustvo);
            _dbContext.SaveChanges();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.PlaninarskoDrustvo> response = new ApiResponse<Models.Data.PlaninarskoDrustvo>();
            var drustvo = _dbContext.PlaninarskoDrustvo.Include(x=>x.KorisnikAdmin).Where(x => x.Id == id).FirstOrDefault();
            if (drustvo == null)
            {
                response.Success = false;
                response.Message = "Drustvo nije pronadjeno!";
                return BadRequest(response);
            }
            response.Data = drustvo;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.PlaninarskoDrustvo> response = new ApiResponse<Models.Data.PlaninarskoDrustvo>();
            var drustvo= _dbContext.PlaninarskoDrustvo.Where(x => x.Id == id).FirstOrDefault();
            if (drustvo == null)
            {
                response.Success = false;
                response.Message = "Drustvo nije pronadjeno!";
                return BadRequest(response);
            }
            if (drustvo.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Success = false;
                response.Message = "Drustvo nije vase!";
                return BadRequest(response);
            }
            if (drustvo.DrustvoSlika != "")
            {
                var postojeca = drustvo.DrustvoSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaDrustva = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaDrustva);
            }
            var sviClanovi=_dbContext.DrustvoKorisnici.Where(x=>x.PlaninarskoDrustvoID==drustvo.Id).ToList();
            _dbContext.RemoveRange(sviClanovi);
            _dbContext.PlaninarskoDrustvo.Remove(drustvo);
            _dbContext.SaveChanges();
            response.Message = "Drustvo obrisano!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet]
        public List<Models.Data.PlaninarskoDrustvo> GetAll()
        {
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            var drustva = _dbContext.PlaninarskoDrustvo.Take(100).ToList();

            return drustva;
        }
        [HttpPost]
        public IActionResult JoinDrustvo(int drustvoId)
        {
            ApiResponse<Models.Data.PlaninarskoDrustvo> response = new ApiResponse<Models.Data.PlaninarskoDrustvo>();
            var drustvo=_dbContext.PlaninarskoDrustvo.Where(x=>x.Id==drustvoId).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (drustvo == null)
            {
                response.Success = false;
                response.Message = "Trazeno drustvo nije pronadjeno!";
                return BadRequest(response);
            }
            var postojiLi = _dbContext.DrustvoKorisnici.Any(x => x.KorisnikId == korisnik.Id && x.PlaninarskoDrustvoID == drustvo.Id);
            if (postojiLi)
            {
                var zapis = _dbContext.DrustvoKorisnici.Where(x => x.KorisnikId == korisnik.Id && x.PlaninarskoDrustvoID == drustvo.Id).FirstOrDefault();
                _dbContext.Remove(zapis);
                _dbContext.SaveChanges();
                response.Message = "Drustvo uspjesno napusteno!";
                response.Success = true;
                return Ok(response);
            }
            var noviClan = new DrustvoKorisnici
            {
                Korisnik = korisnik,
                KorisnikId = korisnik.Id,
                PlaninarskoDrustvo = drustvo,
                PlaninarskoDrustvoID = drustvo.Id
            };
            _dbContext.DrustvoKorisnici.Add(noviClan);
            _dbContext.SaveChanges();
            response.Message = "Uspjeno ste se uclanili u drustvo!";
            return Ok(response);
        }
        [HttpGet("{drustovId}")]
        public IActionResult GetClanoviDrustva(int drustvoId)
        {
            ApiResponse<List<FIT_Api_Example.Modul.Data.Korisnik>> response = new ApiResponse<List<FIT_Api_Example.Modul.Data.Korisnik>>();
            var drustvo = _dbContext.PlaninarskoDrustvo.Where(x => x.Id == drustvoId).FirstOrDefault();
            if (drustvo == null)
            {
                response.Message = "Trazeno drustvo ne postoji!";
                response.Success = false;
                return BadRequest(response);
            }
            var clanovi = _dbContext.DrustvoKorisnici.Where(x => x.PlaninarskoDrustvoID == drustvo.Id).Select(x => x.Korisnik).ToList();
            response.Data = clanovi;
            return Ok(response);
        }
        [HttpGet("{drustvoNaziv}")]
        public IActionResult GetDrustvoByNaziv(string drustvoNaziv)
        {
            ApiResponse<List<Models.Data.PlaninarskoDrustvo>> response = new ApiResponse<List<PlaninarskoDrustvo>>();
            var drustva = _dbContext.PlaninarskoDrustvo.Where(x => x.Naziv.Contains(drustvoNaziv)).ToList();
            if (drustva.IsNullOrEmpty())
            {
                response.Message = "Trazeno drustvo ne postoji!";
                return Ok(response);
            }
            response.Data = drustva;
            return Ok(response);
        }
    }
}
