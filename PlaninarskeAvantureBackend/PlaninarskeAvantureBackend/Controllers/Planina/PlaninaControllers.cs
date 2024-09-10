using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlaninarskeAvantureBackend.Controllers.Planina.PlaninaValidator;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Planina
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaninaControllers : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly AuthService _authService;
        public PlaninaControllers(ApplicationDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult DodajPlaninu([FromBody]PlaninaAddVM planina)
        {
            ApiResponse<Models.Data.Planina> response = new ApiResponse<Models.Data.Planina>();
            PlaninaCreateValidator validator = new PlaninaCreateValidator();
            ValidationResult result=validator.Validate(planina);
            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (korisnik == null)
            {
                response.Message = "Niste prijavljeni!";
                response.Success = false;
                return BadRequest(response);
            }
            var nova = new Models.Data.Planina
            {
                Naziv = planina.Naziv,
                NajvisiVrh = planina.NajvisiVrh,
                Visina = planina.Visina,
                OpisPlanine = planina.Opis,
                Korisnik=korisnik,
                KorisnikId=korisnik.Id
            };
            _dbContext.Planina.Add(nova);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = "Planina uspjesno dodana!";
            response.Data = nova;
            return Ok(response);
        }
        [HttpPut]
        public IActionResult UrediPlaninu([FromBody] PlaninaEditVM _planina)
        {
            ApiResponse<Models.Data.Planina> response = new ApiResponse<Models.Data.Planina>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var planina = _dbContext.Planina.Where(x => x.Id == _planina.Id).FirstOrDefault();
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste prijavljeni!";
                return BadRequest(response);
            }
            if (planina == null)
            {
                response.Success = false;
                response.Message = "Planina nije pronadjena!";
                return BadRequest(response);
            }
            if (planina.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Nije vasa planina!";
                return BadRequest(response);
            }
            planina.Naziv = _planina.Naziv;
            planina.NajvisiVrh = _planina.NajvisiVrh;
            planina.Visina = _planina.Visina;
            planina.OpisPlanine = _planina.Opis;
            _dbContext.Update(planina);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = "Planina izmjenjena!";
            response.Data = planina;
            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult ObrisiPlaninu(int id)
        {
            ApiResponse<Models.Data.Planina> response = new ApiResponse<Models.Data.Planina>();
            var planina = _dbContext.Planina.Where(x => x.Id == id).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste prijavljeni!";
                return BadRequest(response);
            }
            if (planina == null)
            {
                response.Success = false;
                response.Message = "Planina nije pronadjena!";
                return BadRequest(response);
            }
            if (planina.KorisnikId != korisnik.Id)
            {
                response.Success = false;
                response.Message = "Nije vasa planina!";
                return BadRequest();
            }
            _dbContext.Planina.Remove(planina);
            _dbContext.SaveChanges();
            response.Success = true;
            response.Message = "Planina obrisana!";
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id) {
            ApiResponse<Models.Data.Planina> response = new ApiResponse<Models.Data.Planina>();
            var planina =_dbContext.Planina.Where(x=>x.Id== id).FirstOrDefault();
            if (planina == null)
            {
                response.Success = false;
                response.Message = "Planina nije pronadjena!";
                return BadRequest(response);
            }
            response.Success = true;
            response.Data = planina;
            return Ok(response);
        }
    }
}
