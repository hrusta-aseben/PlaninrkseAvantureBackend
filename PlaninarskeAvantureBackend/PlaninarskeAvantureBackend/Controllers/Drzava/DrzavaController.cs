using FIT_Api_Example.Data;
using FIT_Api_Example.Modul.Data;
using FIT_Api_Example.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using PlaninarskeAvantureBackend.Helper;

namespace FIT_Api_Example.Controllers.Drzava
{
    [ApiController]
    [Route("api/drzava/[action]")]

    public class DrzavaController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authservice;
        public DrzavaController(ApplicationDbContext applicationDbContext, AuthService authService)
        {
            _applicationDbContext = applicationDbContext;
            _authservice = authService;
        }
        [HttpPost]
        public IActionResult dodaj([FromBody] DrzavaAddVM drzava)
        {

            ApiResponse<FIT_Api_Example.Modul.Data.Drzava> response = new ApiResponse<FIT_Api_Example.Modul.Data.Drzava>();

            if (!_authservice.Logiran())
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);
            }


            var novaDrzava = new FIT_Api_Example.Modul.Data.Drzava
            {
                Naziv=drzava.Naziv
            };
            _applicationDbContext.Drzava.Add(novaDrzava);
            _applicationDbContext.SaveChanges();

            response.Data = novaDrzava;
            response.Message = "Uspješno dodana nova država.";

            return new JsonResult(new { novaDrzava });
        }
        [HttpGet]
        public IActionResult get([FromQuery] string Drzava)
        {
            ApiResponse<List<FIT_Api_Example.Modul.Data.Drzava>> response = new ApiResponse<List<FIT_Api_Example.Modul.Data.Drzava>>();

            var drzave =_applicationDbContext.Drzava.Where(x=>x.Naziv.StartsWith(Drzava) || Drzava.IsNullOrEmpty()).ToList();

            if (drzave.Count == 0)
            {
                response.Message = "Doslo je do errora.";
                response.Success = false;
            }

            response.Data = drzave;

            return Ok(response);
        }

        [HttpGet]
        public IActionResult index()
        {
            ApiResponse<List<FIT_Api_Example.Modul.Data.Drzava>> response = new ApiResponse<List<FIT_Api_Example.Modul.Data.Drzava>>();

            var drzave = _applicationDbContext.Drzava.ToList();

            response.Data = drzave;

            if(drzave.Count == 0)
            {
                response.Message = "Doslo je do errora.";
                response.Success = false;
            } 

            return Ok(response);
        }
    }
}
