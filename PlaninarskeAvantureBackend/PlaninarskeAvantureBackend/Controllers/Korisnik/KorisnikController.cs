using Azure.Core;
using FIT_Api_Example.Data;
using FIT_Api_Example.Helper;
using FIT_Api_Example.Helper.Servisi;
using FIT_Api_Example.Modul.Data;
using FIT_Api_Example.ViewModels;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlaninarskeAvantureBackend.Controllers.Korisnik.Validators;
using PlaninarskeAvantureBackend.Controllers.Korisnik.VM;
using PlaninarskeAvantureBackend.Helper;
using PlaninarskeAvantureBackend.Helper.Servisi;
using PlaninarskeAvantureBackend.Models.Data;
using PlaninarskeAvantureBackend.ViewModels;
using System.Security.Cryptography;

namespace PlaninarskeAvantureBackend.Controllers.Korisnik
{
    [Route("api/korisnik/[action]")]
    [ApiController]
    public class KorisnikController : ControllerBase
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly AuthService _authService;
        public EmailSender _sender;

        public KorisnikController(ApplicationDbContext applicationDbContext, AuthService authService, EmailSender sender)
        {
            _applicationDbContext = applicationDbContext;
            _authService = authService;
            _sender = sender;
        }
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        [HttpPost]
        public IActionResult login([FromBody] KorisnikLoginRequest request)
        {
            ApiResponse<KorisnikLoginResponse> response = new ApiResponse<KorisnikLoginResponse>();

            var user = _applicationDbContext.Korisnici.Where(x => x.Email == request.Email).FirstOrDefault();

            if (user == null)
            {
                response.Message = "Unijeli ste pogrešne podatke.";
                response.Success = false;
                return BadRequest(response);
            }

            var lozinka = user.Lozinka;

            byte[] hashBytes = Convert.FromBase64String(lozinka);
            byte[] salt = new byte[16];

            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(request.Lozinka, salt, 100000);

            byte[] hash = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                {
                    response.Message = "Unijeli ste pogrešne podatke.";
                    response.Success = false;
                    return BadRequest(response);
                }
            }
            var randomS = GenerisiJwtToken.GenerisiToken();

            var noviToken = new AutentifikacijaToken
            {
                vrijednost = randomS,
                Korisnik = user,
                vrijemeEvidentiranja = DateTime.Now
            };

            _applicationDbContext.AutentifikacijaToken.Add(noviToken);
            _applicationDbContext.SaveChanges();

            response.Data = new KorisnikLoginResponse { Korisnik = user, Token = noviToken.vrijednost };

            if (user.jelPotvrdjen == false)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 10000);

                string broj = randomNumber.ToString("D4");
                var subject = "Potvrda mail-a";
                var body = $"Vas verifikacijski kod za potvrdu mail-a je: {broj}";
                 _sender.SendEmailAsync(user.Email, subject, body);
                var postojeci = _applicationDbContext.VerifikacijskiKodovi.Where(x => x.Email == user.Email).FirstOrDefault();
                postojeci.Kod = broj;
                _applicationDbContext.VerifikacijskiKodovi.Update(postojeci);
                _applicationDbContext.SaveChanges();
                return Ok( "Niste potvrdili account, molimo provjerite mail za autorizacijski kod.");
            }
            var trenutnoVrijeme = DateTime.Now;
            var twoFa = _applicationDbContext.KorisnikFA.Where(x => x.KorisnikId == user.Id).FirstOrDefault();
            if (twoFa != null)
            {

            if (trenutnoVrijeme > twoFa.DatumIsteka)
            {
                twoFa.isAktivan = false;
                user.isAktivan = false;
                _applicationDbContext.Update(user);
                _applicationDbContext.Update(twoFa);
                _applicationDbContext.SaveChanges();
            }

            if (user.TWOFA == true && twoFa.isAktivan == false && trenutnoVrijeme > twoFa.DatumIsteka)
            {
                Random random = new Random();
                int randomNumber = random.Next(0, 10000);

                string broj = randomNumber.ToString("D4");
                var subject = "Autorizacijski kod:";
                var body = $"Vas autorizacijski kod za nastavak je: {broj}";

                var postojeci = _applicationDbContext.KorisnikFA.Where(x => x.KorisnikId == user.Id).FirstOrDefault();
                postojeci.isAktivan = false;
                postojeci.Kod = broj;
                _applicationDbContext.Update(postojeci);
                _applicationDbContext.SaveChanges();
                response.Message = "Provjerite mail za 2FA kod i unesite ga: ";
                _sender.SendEmailAsync(user.Email, subject, body);
                response.Data.Token = noviToken.vrijednost;
                return Ok(response);
            }
            }


            return Ok(response);
        }
        [HttpPost("{kod}")]
        public IActionResult AutorizacijskiKod(string kod)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var zapis = _applicationDbContext.KorisnikFA.Where(x => x.KorisnikId == korisnik.Id).FirstOrDefault();
            if (kod != zapis.Kod)
            {
                response.Message = "Unjeli ste pogresan kod!";
                return Ok(response);
            }
            zapis.isAktivan = true;
            korisnik.isAktivan = true;
            zapis.DatumIsteka=DateTime.Now.AddDays(1);
            _applicationDbContext.Update(korisnik);
            _applicationDbContext.Update(zapis);
            _applicationDbContext.SaveChanges();
            response.Message = "Tacan unos!";
            return Ok(response);
        }
        [HttpPost]
        public IActionResult logout()
        {
            ApiResponse<string> response = new ApiResponse<string>();

            AutentifikacijaToken? autentifikacijaToken = _authService.GetInfo().autentifikacijaToken;

            if (autentifikacijaToken == null)
            {
                response.Success = false;
                response.Message = "Niste ulogovani.";
                return BadRequest(response);
            }

            _applicationDbContext.AutentifikacijaToken.Remove(autentifikacijaToken);
            _applicationDbContext.SaveChanges();

            response.Success = true;
            response.Message = "Uspješno ste se odjavili.";

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> register([FromBody] KorisnikAddVM request)
        {
            ApiResponse<KorisnikDodajResponse> response = new ApiResponse<KorisnikDodajResponse>();

            KorisnikRegistracijaValidator validator = new KorisnikRegistracijaValidator();

            ValidationResult result = validator.Validate(request);

            if (!result.IsValid)
            {
                response.Message = result.ToString();
                response.Success = false;
                return BadRequest(response);
            }

            if (request.Lozinka != request.PotvrdaLozinke)
            {
                response.Message = "Potvrda lozinke se ne podudara sa lozinkom.";
                response.Success = false;
                return BadRequest(response);
            }

            bool postojiLiIsti = _applicationDbContext.Korisnici.Any(x => x.Email == request.Email);

            if (!IsValidEmail(request.Email))
            {
                response.Message = "Neispravna e-mail adresa.";
                return BadRequest(response);
            }

            if (postojiLiIsti)
            {
                response.Message = "Postoji nalog sa istom email adresom.";
                return BadRequest(response);
            }

            byte[] salt;

            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);

            var pbkdf2 = new Rfc2898DeriveBytes(request.Lozinka, salt, 100000);

            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];

            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);

            string lozinka = Convert.ToBase64String(hashBytes);

            var korisnik = new FIT_Api_Example.Modul.Data.Korisnik
            {
                Ime = request.Ime,
                Prezime = request.Prezime,
                Email = request.Email,
                DatumRodjenja = request.DatumRodjenja,
                Lozinka = lozinka,
                Drzava = request.Drzava,
                SlikaKorisnika = "",
                jelPotvrdjen = false
            };
            var noviK = new AutentifikacijaToken
            {
                Korisnik = korisnik,
                vrijednost = GenerisiJwtToken.GenerisiToken(),
                vrijemeEvidentiranja = DateTime.Now,
                KorisnikID = korisnik.Id
            };

            _applicationDbContext.AutentifikacijaToken.Add(noviK);
            _applicationDbContext.Korisnici.Add(korisnik);

            await _applicationDbContext.SaveChangesAsync();
            Random random = new Random();
            int randomNumber = random.Next(0, 10000);

            string broj = randomNumber.ToString("D4");
            var subject = "Potvrda mail-a";
            var body = $"Vas verifikacijski kod za potvrdu mail-a je: {broj}";
            await _sender.SendEmailAsync(korisnik.Email, subject, body);

            var noviKod = new VerifikacijskiKodovi
            {
                Email = korisnik.Email,
                Kod = broj
            };
            _applicationDbContext.VerifikacijskiKodovi.Add(noviKod);
            _applicationDbContext.SaveChanges();

            response.Data = new KorisnikDodajResponse
            {
                Ime = korisnik.Ime,
                Prezime = korisnik.Prezime,
                Token = noviK.vrijednost
            };


            response.Success = true;

            response.Message = "Uspješna registracija.";

            return Ok(response);
        }

        [HttpGet]
        public IActionResult verifikacija()
        {
            ApiResponse<FIT_Api_Example.Modul.Data.Korisnik> response = new ApiResponse<FIT_Api_Example.Modul.Data.Korisnik>();

            var k = _authService.GetInfo().autentifikacijaToken;

            if (k == null)
            {
                response.Success = false;
                response.Message = "Niste prijavljeni.";

                return BadRequest(response);
            }

            response.Success = true;
            response.Data = k.Korisnik;

            return Ok(response);
        }
        [HttpPut]
        public IActionResult Uredi([FromForm] KorisnikUrediVM _korisnik)
        {
            ApiResponse<FIT_Api_Example.Modul.Data.Korisnik> response = new ApiResponse<FIT_Api_Example.Modul.Data.Korisnik>();

            var k = _authService.GetInfo().korisnickiNalog;
            var staraVrijednost = k.TWOFA;
            if (k == null)
            {
                response.Success = false;
                response.Message = "Niste prijavljeni.";

                return BadRequest(response);
            }

            if (_korisnik.Ime != k.Ime || _korisnik.Prezime != k.Prezime ||
                _korisnik.DatumRodjenja != k.DatumRodjenja || _korisnik.Drzava != k.Drzava)
            {
                var lozinka = k.Lozinka;

                byte[] hashBytes = Convert.FromBase64String(lozinka);
                byte[] salt = new byte[16];

                Array.Copy(hashBytes, 0, salt, 0, 16);

                var pbkdf2 = new Rfc2898DeriveBytes(_korisnik.PotvrdaLozinke, salt, 100000);

                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        response.Message = "Unijeli ste pogrešnu lozinku.";
                        response.Success = false;
                        return BadRequest(response);
                    }
                }
            }
            k.Ime = _korisnik.Ime;
            k.Prezime = _korisnik.Prezime;
            k.DatumRodjenja = _korisnik.DatumRodjenja;
            k.Drzava = _korisnik.Drzava;
            k.TWOFA = _korisnik.TWOFA;
            if (k.SlikaKorisnika != "")
            {
                var postojeca = k.SlikaKorisnika;
                string[] parts = postojeca.Split('/');
                string imeFajla = parts[^1];

                var slikaKorisnika = Config.SlikeFolder + imeFajla;
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.IO.File.Delete(slikaKorisnika);
            }
            string ekstenzija = Path.GetExtension(_korisnik.SlikaKorisnika.FileName);
            var fileName = $"{Guid.NewGuid()}{ekstenzija}";
            _korisnik.SlikaKorisnika.CopyTo(new FileStream(Config.SlikeFolder + fileName, FileMode.Create));
            k.SlikaKorisnika = Config.SlikeURL + fileName;
            _applicationDbContext.Update(k);
            _applicationDbContext.SaveChanges();
            if (staraVrijednost==false&& k.TWOFA == true)
            {
                var novi2FA = new KorisnikFA
                {
                    DatumIsteka = DateTime.Now,
                    Korisnik = k,
                    KorisnikId = k.Id,
                    isAktivan = false
                };
                k.isAktivan = true;
                _applicationDbContext.Update(k);
                _applicationDbContext.KorisnikFA.Add(novi2FA);
                _applicationDbContext.SaveChanges();
            }
            response.Data = k;
            response.Success = true;
            return Ok(response);
        }

        [HttpPost("{kod}")]
        public IActionResult VerifikacijskiKod(string kod)
        {
            ApiResponse<string> response = new ApiResponse<string>();
            var korisnik = _authService.GetInfo().korisnickiNalog;
            var mail = korisnik.Email;
            var spremljeni = _applicationDbContext.VerifikacijskiKodovi.Where(x => x.Email == mail).Select(x => x.Kod).FirstOrDefault();
            if (spremljeni != kod)
            {
                response.Message = "Unjeli ste pogresan kod!";
                response.Success = false;
                return BadRequest(response);
            }
            korisnik.jelPotvrdjen = true;
            _applicationDbContext.Update(korisnik);
            _applicationDbContext.SaveChanges();
            response.Success = true;
            response.Message = "Uspjesno potvrdjen account!";
            return Ok(response.Message);
        }


        [HttpGet("{id}")]
        public IActionResult GetPostByKorisnik(int id)
        {
            ApiResponse<KorisnikGetPostovi> response = new ApiResponse<KorisnikGetPostovi>();
            var korisnik = _applicationDbContext.Korisnici.Where(x => x.Id == id).FirstOrDefault();
            if (korisnik == null)
            {
                response.Success = false;
                response.Message = "Trazeni korisnik ne postoji!";
                return BadRequest(response);
            }

            var postovi = _applicationDbContext.Post.Include(x => x.Lajkovi).Where(x => x.KorisnikId == korisnik.Id).OrderByDescending(x=>x.DatumKreiranja).ToList();
            if (postovi.IsNullOrEmpty())
            {
                response.Success = false;
                response.Message = "Korisnik nema postova!";
                return BadRequest(response);
            }
            foreach (var p in postovi)
            {
                p.BrojLajkova = _applicationDbContext.PostLajkovi.Where(x => x.PostId == p.Id).Count();
                var a = p.Komentari = _applicationDbContext.Komentar.Include(x => x.Lajkovi).Include(x => x.Korisnik).Where(x => x.PostId == p.Id).ToList();
                foreach (var pe in a)
                {
                    pe.LikeCounter = _applicationDbContext.KomentarLajkovi.Include(x => x.Korisnik).Where(x => x.KomentarID == pe.Id).Count();
                    _applicationDbContext.SaveChanges();
                }
                _applicationDbContext.SaveChanges();
            }
            var lista = new KorisnikGetPostovi
            {
                Korisnik = korisnik,
                Postovi = postovi
            };
            response.Data = lista;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetOpremaByUser(int id)
        {
            ApiResponse<List<Models.Data.Oprema>> response = new ApiResponse<List<Models.Data.Oprema>>();
            List<Models.Data.Oprema> oprema = new List<Models.Data.Oprema>();
            oprema = _applicationDbContext.Oprema.Where(x => x.Id == id).ToList();
            if (oprema.IsNullOrEmpty())
            {
                response.Message = "Trazeni korisnik name opreme!";
                return Ok(response.Message);
            }
            response.Success = true;
            response.Data = oprema;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetStazaByUser(int id)
        {
            ApiResponse<List<Models.Data.Staza>> response = new ApiResponse<List<Models.Data.Staza>>();
            var staze = _applicationDbContext.Staza.Include(x=>x.Planina).Include(x=>x.Korisnik).Where(x => x.KorisnikId == id).OrderByDescending(x=>x.VrijemeKreiranja).ToList();
            if (staze.IsNullOrEmpty())
            {
                response.Message = "Korisnik nije napravio niti jednu stazu!";
                return Ok(response);
            }
            foreach(var s in staze)
            {
                s.Ocjena = _applicationDbContext.OcjenaStaze.Where(x => x.StazaId == s.Id).Average(x => x.Ocjena);
                _applicationDbContext.Update(s);
                _applicationDbContext.SaveChanges();
            }
            response.Data = staze;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetPutopisiByUser(int id)
        {
            ApiResponse<List<PutopisGetStaze>> response = new ApiResponse<List<PutopisGetStaze>>();
            var putopisi = _applicationDbContext.Putopis.Where(x => x.KorisnikId == id).OrderByDescending(x => x.DatumObjavljivanja).ToList();
            List<PutopisGetStaze> lista = new List<PutopisGetStaze>();
            foreach (var p in putopisi)
            {
                var novi = new PutopisGetStaze
                {
                    Putopis = p,
                    Staza = _applicationDbContext.PutopisStaze.Where(x => x.PutopisId == p.Id).Select(x => x.Staza).ToList()
                };
                lista.Add(novi);
                
            }
            response.Data = lista;


            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetVodicByUser(int id)
        {
            ApiResponse<Models.Data.Vodic> response = new ApiResponse<Models.Data.Vodic>();
            var vodic = _applicationDbContext.Vodic.Where(x => x.KorisnikId == id).FirstOrDefault();
            if (vodic == null)
            {
                response.Message = "Trazeni korisnik nema vodica!";
                return Ok(response);
            }
            response.Data = vodic;
            return Ok(response);
        }
        [HttpGet("{id}")]
        public IActionResult GetEventByUser(int id)
        {
            ApiResponse<List<Models.Data.Dogadjaj>> response = new ApiResponse<List<Models.Data.Dogadjaj>>();
            var eventi = _applicationDbContext.Dogadjaj.Where(x => x.KorisnikId == id).ToList();
            if (eventi.IsNullOrEmpty())
            {
                response.Message = "Trazeni korisnik nije organizovao evente!";
                response.Success = false;
                return BadRequest(response);
            }
            response.Data = eventi;
            return Ok(response);
        }
    }
}
