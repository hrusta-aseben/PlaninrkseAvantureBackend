using FIT_Api_Example.Data;
using FIT_Api_Example.Helper.Servisi;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlaninarskeAvantureBackend.Controllers.Post.Validators;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.ViewModels;

namespace PlaninarskeAvantureBackend.Controllers.Post
{
    [Route("api/post/[action]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authservice;
        public PostController(ApplicationDbContext applicationDbContext, AuthService authService)
        {
            _applicationDbContext = applicationDbContext;
            _authservice = authService;
        }

        [HttpPost]
        public IActionResult dodaj([FromBody] PostAddVM post)
        {
            ApiResponse<Modul.Data.Post> response = new ApiResponse<Modul.Data.Post>();
            PostCreateValidator validator = new PostCreateValidator();

            ValidationResult result = validator.Validate(post);

            if(!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;

                return BadRequest(response);
            }

            if (!_authservice.Logiran())
            {
                response.Message = "Niste ulogovani.";
                response.Success = false;

                return BadRequest(response);
            }

            var korisnik = _authservice.GetInfo().korisnickiNalog;
            var noviPost = new Modul.Data.Post
            {
                Tekst = post.Tekst,
                Korisnik = korisnik,
                Tezina = post.Tezina,
                Vrijeme = post.Vrijeme,
                DatumKreiranja=DateTime.Now
            };

            _applicationDbContext.Post.Add(noviPost);
            _applicationDbContext.SaveChanges();
            

            response.Success = true;
            response.Data = noviPost;

            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult get(int id)
        {
            ApiResponse<Modul.Data.Post> response = new ApiResponse<Modul.Data.Post>();

            var novi = _applicationDbContext.Post.Include(x => x.Korisnik).Include(x => x.Lajkovi).Include(x => x.Komentari)
                .ThenInclude(x => x.Lajkovi).ThenInclude(x => x.Korisnik).Where(x => x.Id == id).FirstOrDefault();

            if (novi == null)
            {
                response.Success=false;
                response.Message = "Nije pronadjen nijedan post sa tim id-om.";

                return BadRequest(response);
            }
            else
            {
                novi.BrojLajkova = _applicationDbContext.PostLajkovi.Where(x => x.PostId == id).Count();
            }

            response.Data = novi;

            return Ok(response);
        }
        [HttpPost("{id}")]
        public IActionResult react(int id)
        {
            ApiResponse<string> response = new ApiResponse<string>();

            var post = _applicationDbContext.Post.Where(x => x.Id == id).FirstOrDefault();
            var korisnik = _authservice.GetInfo().korisnickiNalog;

            if(korisnik == null)
            {
                response.Success=false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);
            }

            var NoviLajk = new Modul.Data.PostLajkovi
            {
                Post = post,
                Korisnik = korisnik
            };

            var isLiked = _applicationDbContext.PostLajkovi.Any(x => x.KorisnikId == korisnik.Id
            && x.PostId == post.Id);

            if (isLiked)
            {
                var lajkanPost = _applicationDbContext.PostLajkovi.Where(x => x.KorisnikId == korisnik.Id && x.PostId == post.Id).FirstOrDefault();
                _applicationDbContext.PostLajkovi.Remove(lajkanPost);
                
                post.Lajkovi.Remove(lajkanPost);
                
                _applicationDbContext.SaveChanges();

                response.Message = "Post odlajkan.";

            }
            else
            {
                _applicationDbContext.PostLajkovi.Add(NoviLajk);
                _applicationDbContext.SaveChanges();

                response.Message = "Post lajkovan.";

            }
            
            return Ok(response);
        }
        [HttpGet]
        public IActionResult index()
        {
            ApiResponse<List<Modul.Data.Post>> response = new ApiResponse<List<Modul.Data.Post>>();

            var sve = _applicationDbContext.Post.Include(x => x.Korisnik)
                .Include(x => x.Lajkovi).Include(x => x.Komentari).ThenInclude(x => x.Lajkovi).ThenInclude(x => x.Korisnik).ToList();

            foreach (var s in sve)
                s.BrojLajkova = _applicationDbContext.PostLajkovi.Where(x => x.PostId == s.Id).Count();

            response.Data = sve;

            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult komentar(int id)
        {
            ApiResponse<List<PostGetKomentarVM>> response = new ApiResponse<List<PostGetKomentarVM>>();

            var komentari = _applicationDbContext.Komentar
                .Include(x => x.Korisnik)
                .Where(x => x.PostId == id)
                .Select(x => new PostGetKomentarVM
                {
                    Id=x.Id,
                    Korisnik= x.Korisnik,
                    Tekst=x.TekstKomentara
                })  
                .ToList(); 
            
            if (komentari==null)
            {
                response.Success = false;
                response.Message = "Nisu pronađeni komentari za traženi post.";

                return BadRequest(response);
            }
            foreach(var k in komentari)
            {
                k.LikeCounter=_applicationDbContext.KomentarLajkovi.Where(x=>x.Id== k.Id).Count();
            }

            response.Data = komentari;

            return Ok(response);
        }
        [HttpDelete("{id}")]
        public IActionResult obrisi(int id)
        {
            ApiResponse<List<PostGetKomentarVM>> response = new ApiResponse<List<PostGetKomentarVM>>();

            var post =_applicationDbContext.Post.Where(x=>x.Id== id).FirstOrDefault();
            
            var korisnik = _authservice.GetInfo().korisnickiNalog;
            
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);

            }
            if (post.KorisnikId == korisnik.Id)
            {
                var lajkovi = _applicationDbContext.PostLajkovi.Where(x => x.PostId == id).ToList();
                _applicationDbContext.PostLajkovi.RemoveRange(lajkovi);
                var komentari = _applicationDbContext.Komentar.Where(x => x.PostId == id).ToList();
                _applicationDbContext.Komentar.RemoveRange(komentari);
                
                _applicationDbContext.SaveChanges();
                _applicationDbContext.Post.Remove(post);
                _applicationDbContext.SaveChanges();
            }
            else
            {
                response.Success = false;
                response.Message = "Nije vaš post.";

                return BadRequest(response);

            }

            response.Message = "Post obrisan uspješno.";

            return Ok(response);
        }
        [HttpPut]
        public IActionResult uredi(UrediPostVM post)
        {
            ApiResponse<Modul.Data.Post> response = new ApiResponse<Modul.Data.Post>();

            var _post = _applicationDbContext.Post.Where(x => x.Id == post.PostID).FirstOrDefault();
            var korisnik = _authservice.GetInfo().korisnickiNalog;

            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";

                return BadRequest(response);
            }


            if (_post == null)
            {
                response.Success = false;
                response.Message = "Post nije pronađen.";

                return BadRequest(response);
            }

            if (_post.KorisnikId == korisnik.Id) { 
                _post.Tekst = post.Tekst ?? _post.Tekst;
                _post.Vrijeme = post.Vrijeme ?? _post.Vrijeme;
                _post.Tezina = post.Tezina ?? _post.Tezina;
                _applicationDbContext.Update(_post);
                _applicationDbContext.SaveChanges();
            }
            else
            {
                response.Success = false;
                response.Message = "Post nije vaš.";

                return BadRequest(response);
            }

            response.Data = _post;
            response.Message = "Uspješno uređen post.";

            return new JsonResult(new { poruka = "Uredjen post" });
        }
    }
}
