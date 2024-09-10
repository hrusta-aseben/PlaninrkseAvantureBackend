using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.Modul.Data;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Komentar
{
    [Route("api/komentar/[action]")]

    public class KomentarController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authService;
        public KomentarController(ApplicationDbContext applicationDbContext, AuthService authService)
        {
            _applicationDbContext = applicationDbContext;
            _authService = authService;
        }
        [HttpPost]
        public IActionResult dodaj([FromBody]KomentarAddVM kom)
        {
            ApiResponse<Modul.Data.Komentar> response = new ApiResponse<Modul.Data.Komentar>();

            var post =_applicationDbContext.Post.Where(x=>x.Id==kom.PostID).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;

            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);
            }

            var noviKomentar = new Modul.Data.Komentar
            {
                Korisnik = korisnik,
                Post = post,
                PostId=post.Id,
                TekstKomentara = kom.TekstKomentara,
                DatumKreiranja=DateTime.Now
            };
            
            _applicationDbContext.Komentar.Add(noviKomentar);
            
            var aaa=_applicationDbContext.Post.Where(x=>x.Id==noviKomentar.PostId).FirstOrDefault();

            aaa.Komentari.Add(noviKomentar);
            _applicationDbContext.SaveChanges();


            response.Data = noviKomentar;
            response.Message = "Uspješno dodan komentar.";


            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            ApiResponse<Modul.Data.Komentar> response = new ApiResponse<Modul.Data.Komentar>();

            var komentar =_applicationDbContext.Komentar.Include(x=>x.Lajkovi).Where(x=>x.Id== id).FirstOrDefault();

            if (komentar == null)
            {
                response.Success = false;
                response.Message = "Komentar nije pronađen.";

                return BadRequest(response);
            }

            komentar.LikeCounter = _applicationDbContext.KomentarLajkovi.Where(x => x.KomentarID == id).Count();

            response.Data = komentar;

            return Ok(response);
        }
        [HttpPost("{id}")]
        public IActionResult react(int id)
        {

            ApiResponse<string> response = new ApiResponse<string>();


            var komentar =_applicationDbContext.Komentar.Where(x=>x.Id== id).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            if (komentar == null)
            {
                response.Success = false;
                response.Message = "Komentar nije pronađen.";

                return BadRequest(response);
            }

            var noviLike = new KomentarLajkovi
            {
                Komentar = komentar,
                KomentarID = komentar.Id,
                Korisnik = korisnik,
                KorisnikId = korisnik.Id
            };

            var isLiked=_applicationDbContext.KomentarLajkovi.Any(x=>x.KomentarID==komentar.Id &&
            x.KorisnikId==korisnik.Id);

            if (isLiked)
            {
                var lajkanKomentar = _applicationDbContext.KomentarLajkovi.Where(x => x.KorisnikId == korisnik.Id &&
                x.KomentarID == komentar.Id).FirstOrDefault();
                _applicationDbContext.KomentarLajkovi.Remove(lajkanKomentar);
                komentar.Lajkovi.Remove(lajkanKomentar);
                _applicationDbContext.SaveChanges();

                response.Message = "Komentar odlajkan.";

            }
            else
            {
                _applicationDbContext.KomentarLajkovi.Add(noviLike);
                komentar.Lajkovi.Add(noviLike);
                _applicationDbContext.SaveChanges();

                response.Message = "Komentar lajkovan.";
            }

            return Ok(response);
        }
        [HttpPut]
        public IActionResult uredi([FromBody]KomentarUrediVM komentar)
        {

            ApiResponse<Modul.Data.Komentar> response = new ApiResponse<Modul.Data.Komentar>();

            var trazeniKom = _applicationDbContext.Komentar.Where(x => x.Id == komentar.KomentarID).FirstOrDefault();
            var korisnik = _authService.GetInfo().korisnickiNalog;

            if (trazeniKom.KorisnikId != korisnik.Id) {

                response.Success = false;
                response.Message = "Komentar nije vaš.";

                return BadRequest(response);
            }

            trazeniKom.TekstKomentara = komentar.Tekst?? trazeniKom.TekstKomentara;
            trazeniKom.DatumKreiranja = komentar.Vrijeme;

            _applicationDbContext.Komentar.Update(trazeniKom);
            _applicationDbContext.SaveChanges();

            response.Data = trazeniKom;

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult obrisi(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var komentar = _applicationDbContext.Komentar.Where(x => x.Id == id).FirstOrDefault();
            var user = _authService.GetInfo().korisnickiNalog;


            if (komentar == null)
            {
                response.Success = false;
                response.Message = "Komentar nije pronađen.";

                return BadRequest(response);
            }

            if (komentar.KorisnikId != user.Id)
            {
                response.Success = false;
                response.Message = "Komentar nije vaš.";

                return BadRequest(response);
            }
            else
            {
                var lajkovi = _applicationDbContext.KomentarLajkovi.Where(x => x.KomentarID == id).ToList();
                _applicationDbContext.KomentarLajkovi.RemoveRange(lajkovi);
                _applicationDbContext.SaveChanges();
                _applicationDbContext.Komentar.Remove(komentar);
                _applicationDbContext.SaveChanges();
            }

            response.Message = "Komentar uspješno obrisan.";

            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult user(int id)
        {
            ApiResponse<List<Modul.Data.Komentar>> response = new ApiResponse<List<Modul.Data.Komentar>>();

            var korisnik = _applicationDbContext.Korisnici.Where(x => x.Id == id).FirstOrDefault();

            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);
            }

            var komentari = _applicationDbContext.Komentar.Include(x => x.Post).Include(x=>x.Lajkovi).Where(x => x.KorisnikId == korisnik.Id).ToList();

            response.Data = komentari;

            return Ok(response);
        }
    }
}
