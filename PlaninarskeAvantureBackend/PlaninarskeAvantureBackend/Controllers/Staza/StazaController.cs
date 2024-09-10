using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlaninarskeAvantureBackend.Controllers.Staza.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using FIT_Api_Example.Helper;
using System.IO;
using Microsoft.IdentityModel.Tokens;

namespace PlaninarskeAvantureBackend.Controllers.Staza
{
    [Route("api/staza/[action]")]
    [ApiController]
    public class StazaController : ControllerBase
    {
        private readonly ApplicationDbContext _appliationDbContext;
        private readonly AuthService _authService;
        public StazaController(ApplicationDbContext appliationDbContext, AuthService authService)
        {
            _appliationDbContext = appliationDbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult Kreiranje([FromForm] StazaAddVM staza)
        {
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            StazaAddValidator validator = new StazaAddValidator();
            ValidationResult result = validator.Validate(staza);
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
            if (staza.SlikaStaze.Length > 300 * 1000)
            {
                response.Message = "Maksimalna velicina fajla je 300 KB";
                response.Success = false;
                return BadRequest(response);
            }
            string ekstenzija = Path.GetExtension(staza.SlikaStaze.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            staza.SlikaStaze.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var novaStaza = new Models.Data.Staza
            {
                Naziv = staza.Naziv,
                Tezina = staza.Tezina,
                Duzina = staza.Duzina,
                NadmorskaVisina = staza.NadmorskaVisina,
                Opis = staza.Opis,
                Lokacija = staza.Lokacija,
                pocetakX = staza.pocetakX,
                pocetakY = staza.pocetakY,
                KorisnikId = korisnik.Id,
                VrijemeKreiranja = DateTime.Now,
                krajX = staza.krajX,
                krajY = staza.krajY,
                PlaninaId = staza.PlaninaId
            };
            novaStaza.SlikaStaze = Config.SlikeURL + fileName;
            _appliationDbContext.Staza.Add(novaStaza);
            _appliationDbContext.SaveChanges();
            response.Success = true;
            response.Data = novaStaza;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult Uredi([FromForm] StazaUrediVM _staza) {

            var staza = _appliationDbContext.Staza.Where(x => x.Id == _staza.StazaID).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            if (staza == null)
            {
                response.Success = false;
                response.Message = "Staza nije pronadjena";
                return BadRequest(response);
            }
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani!";
                return BadRequest(response);
            }
            if (staza.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Nije vasa staza!";
                return BadRequest(response);
            }
            staza.Naziv = _staza.Naziv;
            staza.Tezina = _staza.Tezina;
            staza.Duzina = _staza.Duzina;
            staza.NadmorskaVisina = _staza.NadmorskaVisina;
            staza.Opis = _staza.Opis;
            staza.Lokacija = _staza.Lokacija;
            staza.pocetakX = _staza.pocetakX;
            staza.pocetakY = _staza.pocetakY;
            staza.krajX = _staza.krajX;
            staza.krajY = _staza.krajY;

            if (staza.SlikaStaze != "")
            {
                var postojeca = staza.SlikaStaze;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaStaze = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaStaze);
            }

            string ekstenzija = Path.GetExtension(_staza.Slika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _staza.Slika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            staza.SlikaStaze = Config.SlikeURL + fileName;

            response.Data = staza;
            response.Success = true;
            _appliationDbContext.Staza.Update(staza);
            _appliationDbContext.SaveChanges();
            return Ok(response);

        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            var staza = _appliationDbContext.Staza.Include(x=>x.Planina).Include(x => x.Korisnik).Where(x => x.Id == id).FirstOrDefault();
            if (staza == null)
            {
                response.Success = false;
                response.Message = "Staza nije pronadjena!";
                return BadRequest(response);
            }
            var postojiLi = _appliationDbContext.OcjenaStaze.Any(x => x.StazaId == staza.Id);
            if (postojiLi)
            {
                var suma = _appliationDbContext.OcjenaStaze.Where(x => x.StazaId == id).Average(x => x.Ocjena);
                staza.Ocjena = suma;
                _appliationDbContext.Update(staza);
                _appliationDbContext.SaveChanges();
            }
            response.Data = staza;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult Obrisi(int id)
        {
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            var staza = _appliationDbContext.Staza.Where(x => x.Id == id).FirstOrDefault();
            if (staza == null)
            {
                response.Success = false;
                response.Message = "Dogadjaj nije pronadjen!";
                return BadRequest(response);
            }
            if (staza.KorisnikId != _authService.GetInfo().korisnickiNalog.Id)
            {
                response.Success = false;
                response.Message = "Staza nije vasa!";
                return BadRequest(response);
            }

            var lista = _appliationDbContext.OcjenaStaze.Where(x => x.StazaId == id).ToList();
            _appliationDbContext.OcjenaStaze.RemoveRange(lista);
            _appliationDbContext.SaveChanges();
            if (staza.SlikaStaze != "")
            {
                var postojeca = staza.SlikaStaze;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaStaze = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaStaze);
            }
            _appliationDbContext.Staza.Remove(staza);
            _appliationDbContext.SaveChanges();
            response.Message = "Staza obrisana!";
            response.Success = true;
            return Ok(response);
        }
        [HttpGet]
        public List<Models.Data.Staza> GetAll()
        {
            ApiResponse<Models.Data.Staza> response = new ApiResponse<Models.Data.Staza>();
            var staze = _appliationDbContext.Staza.OrderBy(x => x.VrijemeKreiranja).Take(100).ToList();
            foreach (var s in staze)
            {
                var postojiLi = _appliationDbContext.OcjenaStaze.Any(x => x.StazaId == s.Id);
                if (postojiLi)
                {
                    s.Ocjena = _appliationDbContext.OcjenaStaze.Where(x => x.StazaId == s.Id).Average(x => x.Ocjena);
                    _appliationDbContext.SaveChanges();
                }
                else
                {
                    s.Ocjena = 0;
                    _appliationDbContext.SaveChanges();
                }
            }
            _appliationDbContext.SaveChanges();
            return staze;
        }
        [HttpGet("{id}")]
        public IActionResult GetOcjeneStaze(int id)
        {
            ApiResponse<List<Models.Data.OcjenaStaze>> response = new ApiResponse<List<Models.Data.OcjenaStaze>>();
            var ocjene = _appliationDbContext.OcjenaStaze.Include(x=>x.Korisnik).Where(x => x.StazaId == id).ToList();
            if (ocjene.IsNullOrEmpty())
            {
                response.Message = "Trazena staza nema ocjena!";
                return Ok(response);
            }

            response.Data = ocjene;
            return Ok(response);
        }
        [HttpGet("planina")]
        public IActionResult GetStazaByPlanina(string planina)
        {
            ApiResponse<List<Models.Data.Staza>> response = new ApiResponse<List<Models.Data.Staza>>();
            var staze = _appliationDbContext.Staza.Include(x=>x.Korisnik).Where(x => planina.Contains(x.Planina.Naziv)).ToList();
            if (staze.IsNullOrEmpty())
            {
                response.Message = "Na trazenoj planini nema staza!";
                return Ok(response);
            }
            response.Data = staze;
            return Ok(response);
        }
        
    }
}
