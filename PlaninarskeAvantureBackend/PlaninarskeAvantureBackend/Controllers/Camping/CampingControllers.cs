using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FIT_Api_Example.Modul.Data;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlaninarskeAvantureBackend.Controllers.Camping.CampingValidators;
using PlaninarskeAvantureBackend.Controllers.Staza.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.Models.Data;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Camping
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CampingControllers : ControllerBase
    {
        private readonly ApplicationDbContext _appliationDbContext;
        private readonly AuthService _authService;
        public CampingControllers(ApplicationDbContext appliationDbContext, AuthService authService)
        {
            _appliationDbContext = appliationDbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult DodajCamp([FromForm] CampingAddVM camp)
        {
            ApiResponse<Models.Data.Camping> response = new ApiResponse<Models.Data.Camping>();
            CampingAddValidator validator = new CampingAddValidator();
            ValidationResult result = validator.Validate(camp);
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
            string ekstenzija = Path.GetExtension(camp.SlikaCamp.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            camp.SlikaCamp.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            var noviCamp = new Models.Data.Camping
            {
                Naziv = camp.Naziv,
                Tip = camp.Tip,
                CijenaPoNoci = camp.CijenaPoNoci,
                Kontakt = camp.Kontakt,
                X = camp.X,
                Y = camp.Y,
                Korisnik=korisnik,
                KorisnikId=korisnik.Id,
                CampSlika=Config.SlikeURL+fileName,
                Lokacija=camp.Lokacija
            };
            _appliationDbContext.Camping.Add(noviCamp);
            _appliationDbContext.SaveChanges();
            response.Data = noviCamp;
            response.Success = true;
            response.Message = "Camp uspjesno dodan!";
            return Ok(response);
        }
        [HttpPut]
        public IActionResult EditCamp([FromForm] CampingEditVM _camp)
        {
            ApiResponse<Models.Data.Camping> response = new ApiResponse<Models.Data.Camping>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var camp = _appliationDbContext.Camping.Where(x => x.Id == _camp.Id).FirstOrDefault();
            if (camp == null)
            {
                response.Message = "Trazeno kamping mjesto nije pronadjeno!";
                response.Success = false;
                return BadRequest(response);
            }
            if (korisnik.Id != camp.KorisnikId)
            {
                response.Message = "Trazeno kamping mjesto ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            camp.Naziv = _camp.Naziv;
            camp.Kontakt = _camp.Kontakt;
            camp.CijenaPoNoci = _camp.CijenaPoNoci;
            camp.Tip = _camp.Tip;
            camp.X = _camp.X;
            camp.Y = _camp.Y;
            if (camp.CampSlika!= "")
            {
                var postojeca = camp.CampSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaCampa = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaCampa);
            }
            string ekstenzija = Path.GetExtension(_camp.CampSlika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _camp.CampSlika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            camp.CampSlika= Config.SlikeURL + fileName;
            _appliationDbContext.Camping.Update(camp);
            _appliationDbContext.SaveChanges();
            response.Message = "Trazeno mjesto uspjesno izmjenjeno!";
            response.Success = true;
            response.Data = camp;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetCampById(int id)
        {
            ApiResponse<Models.Data.Camping> response = new ApiResponse<Models.Data.Camping>();
            var camp = _appliationDbContext.Camping.Where(x => x.Id == id).FirstOrDefault();
            if(camp==null)
            {
                response.Message = "Ne mozemo pronaci trazeno mjesto za kampiranje!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Data = camp;
            response.Success = true;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult ObrisiPoId(int id)
        {
            ApiResponse<Models.Data.Camping> response = new ApiResponse<Models.Data.Camping>();
            var camp = _appliationDbContext.Camping.Where(x => x.Id == id).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (camp == null)
            {
                response.Message = "Ne mozemo pronaci trazeno mjesto za kampiranje!";
                response.Success = false;
                return BadRequest(response);
            }
            if (korisnik.Id != camp.KorisnikId)
            {
                response.Message = "Trazeno kamping mjesto ne pripada vama!";
                response.Success = false;
                return BadRequest(response);
            }
            if (camp.CampSlika != "")
            {
                var postojeca = camp.CampSlika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];
                var slikaCampa = Config.SlikeFolder + imeFajla;

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaCampa);
            }
            _appliationDbContext.Camping.Remove(camp);
            _appliationDbContext.SaveChanges();
            response.Message = "Mjesto za kampiranje uspjesno obrisano!";
            response.Success = true;
            return Ok();
        }
        [HttpGet("{lokacija}")]
        public IActionResult GetCampByLokacija(string lokacija)
        {
            ApiResponse<List<Models.Data.Camping>> response = new ApiResponse<List<Models.Data.Camping>>();
            var camp=_appliationDbContext.Camping.Where(x=>x.Lokacija.Contains(lokacija)).ToList();
            if (camp == null)
            {
                response.Success = false;
                response.Message = "Nisu pronadjeni campovi!";
            }
            response.Success = true;
            response.Data = camp;
            return Ok(response);
        }

    }
}
