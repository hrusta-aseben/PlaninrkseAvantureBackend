using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using Microsoft.AspNetCore.Mvc;
using FIT_Api_Example.Modul.Data;
using PlaninarskeAvantureBackend.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Pracenja
{
    [Route("api/korisnik/[action]")]
    [ApiController]
    public class PracenjaEndpoints : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authservice;
        public PracenjaEndpoints(ApplicationDbContext applicationDbContext, AuthService authservice)
        {
            _applicationDbContext = applicationDbContext;
            _authservice = authservice;
        }
        [HttpPost("{id}")]
        public JsonResult Zaprati(int id)
        {
            var Korisnikuser = _authservice.GetInfo().korisnickiNalog;
            var korisnikZapraceni=_applicationDbContext.Korisnici.Where(x=>x.Id== id).FirstOrDefault();

            if (Korisnikuser == null)
                return new JsonResult(new { poruka = "Niste prijavljeni!" });
            if (korisnikZapraceni == null)
                return new JsonResult(new { poruka = "Nije pornadjen account!" });

            var novoPracenje = new Models.Data.Pracenja
            {
                KorisnikUser = Korisnikuser,
                KorisnikUserId = Korisnikuser.Id,
                KorisnikZapracen = korisnikZapraceni,
                KorsnikZapracenId = korisnikZapraceni.Id
            };
            _applicationDbContext.Pracenja.Add(novoPracenje);
            _applicationDbContext.SaveChanges();
            return new JsonResult(new { poruka = "Korisnik " +Korisnikuser.Ime +" zapratio "+ korisnikZapraceni.Ime });
        }
        [HttpGet]
        public JsonResult ZapraceniPostovi()
        {
            ApiResponse<Models.Data.Pracenja> response = new ApiResponse<Models.Data.Pracenja>();
            var korisnik = _authservice.GetInfo().korisnickiNalog;
            var zapraceni = _applicationDbContext.Pracenja.Where(x => x.KorisnikUserId == korisnik.Id).Select(x => x.KorsnikZapracenId).ToList();
            if (zapraceni == null)
            {
                response.Success= false;
                response.Message = "Niste nikoga zapratili!";
            }
            var post = _applicationDbContext.Post.Include(x=>x.Lajkovi).Where(x => zapraceni.Contains(x.KorisnikId)).OrderByDescending(x => x.DatumKreiranja).ToList();
            foreach(var p in post)
            {
                var nest=p.Komentari = _applicationDbContext.Komentar.Include(x=>x.Korisnik).Include(x=>x.Lajkovi).Where(x => x.PostId == p.Id).ToList();
                foreach(var k in nest)
                {
                    k.LikeCounter=_applicationDbContext.KomentarLajkovi.Where(x=>x.KomentarID== k.Id).Count();
                }
                p.BrojLajkova = _applicationDbContext.PostLajkovi.Where(x => x.PostId == p.Id).Count();
            }
            
            return new JsonResult(new { post });
        }

        [HttpGet]
        public IActionResult ZapraceniEventi()
        {
            ApiResponse<List<Models.Data.Dogadjaj>> response = new ApiResponse<List<Models.Data.Dogadjaj>>();
            var korisnik = _authservice.GetInfo().korisnickiNalog;
            var zapraceni = _applicationDbContext.Pracenja.Where(x => x.KorisnikUserId == korisnik.Id).Select(x=>x.KorsnikZapracenId).ToList();
            if (zapraceni == null)
            {
                response.Message = "Niste nikoga zapratili!";
                response.Success = false;
                return BadRequest(response);
            }
            var datum = DateTime.Now;
            var eventi=_applicationDbContext.Dogadjaj.Include(x=>x.Korisnik).Where(x=> zapraceni.Contains(x.KorisnikId) && datum<x.VrijemePocetka).OrderByDescending(x=>x.VrijemeKreiranja).ToList();
            if (eventi == null)
            {
                response.Message = "Nismo pronasli ni jedan event kai cenat!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Data = eventi;
            return Ok(response);
        }
        [HttpGet]
        public IActionResult ZapraceneStaze()
        {
            ApiResponse<List<Models.Data.Staza>> response = new ApiResponse<List<Models.Data.Staza>>();
            var korisnik = _authservice.GetInfo().korisnickiNalog;
            var zapraceni = _applicationDbContext.Pracenja.Where(x => x.KorisnikUserId == korisnik.Id).Select(x => x.KorsnikZapracenId).ToList();
            if (zapraceni == null)
            {
                response.Message = "Niste nikoga zapratili!";
                response.Success = false;
                return BadRequest(response);
            }
            var staze = _applicationDbContext.Staza.Include(x=>x.Korisnik).Include(x=>x.Planina).Where(x => zapraceni.Contains(x.KorisnikId)).OrderByDescending(x => x.VrijemeKreiranja).ToList();
            if (staze.IsNullOrEmpty())
            {
                response.Message = "Nismo pronasli ni jednu stazu kai cenat!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Data = staze;
            response.Success = true;
            return Ok(response);
        }
        [HttpGet]
        public IActionResult ZapraceniPutopis()
        {
            ApiResponse<List<PutopisGetStaze>> response = new ApiResponse<List<PutopisGetStaze>>();
            var korisnik = _authservice.GetInfo().korisnickiNalog;
            var zapraceni = _applicationDbContext.Pracenja.Where(x => x.KorisnikUserId == korisnik.Id).Select(x => x.KorsnikZapracenId).ToList();
            if (zapraceni == null)
            {
                response.Message = "Niste nikoga zapratili!";
                response.Success = false;
                return BadRequest(response);
            }
            var putopisi = _applicationDbContext.Putopis.Include(x => x.Korisnik).Where(x => zapraceni.Contains(x.KorisnikId)).OrderByDescending(x=>x.DatumObjavljivanja).ToList();

            List<PutopisGetStaze> lista = new List<PutopisGetStaze>();
            foreach(var p in putopisi)
            {
                var novi = new PutopisGetStaze
                {
                    Putopis = p,
                    Staza = _applicationDbContext.PutopisStaze.Where(x => x.PutopisId == p.Id).Select(x=>x.Staza).ToList()
                };
               lista.Add(novi);
            }
            response.Data = lista;
            return Ok(response);
        }

    }
}
