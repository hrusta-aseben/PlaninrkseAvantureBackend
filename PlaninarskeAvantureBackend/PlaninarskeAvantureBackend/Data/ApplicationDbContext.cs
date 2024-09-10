using Microsoft.EntityFrameworkCore;
using FIT_Api_Example.Modul.Data;
using FIT_Api_Example.Helper;
using PlaninarskeAvantureBackend.Modul.Data;
using PlaninarskeAvantureBackend.Models.Data;
using System.Reflection.Metadata;

namespace FIT_Api_Example.Data
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<Korisnik> Korisnici { get; set; }
        public DbSet<Drzava> Drzava { get; set; }
        public DbSet<AutentifikacijaToken> AutentifikacijaToken { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostLajkovi> PostLajkovi { get; set; }
        public DbSet<Komentar> Komentar { get; set; }
        public DbSet<KomentarLajkovi> KomentarLajkovi { get; set; }
        public DbSet<Pracenja> Pracenja { get; set; }
        public DbSet<Staza> Staza { get; set; }
        public DbSet<Dogadjaj> Dogadjaj { get; set; }
        public DbSet<PlaninarskoDrustvo> PlaninarskoDrustvo { get; set; }
        public DbSet <Oprema> Oprema { get; set; }
        public DbSet<Vodic> Vodic { get; set; }
        public DbSet<SigurnosniIzvjestaj> SigurnosniIzvjestaj { get; set; }
        public DbSet<Putopis> Putopis { get; set; }
        public DbSet<PutopisStaze> PutopisStaze { get; set; }
        public DbSet<OcjenaStaze> OcjenaStaze{ get; set; }
        public DbSet<Planina> Planina { get; set; }
        public DbSet<Camping> Camping { get; set; }
        public DbSet<Sponzor> Sponzor { get; set; }
        public DbSet<SigurnosniRizik> SigurnosniRizik { get; set; }
        public DbSet<DrustvoKorisnici> DrustvoKorisnici { get; set; }
        public DbSet<DogadjajKorisnici> DogadjajKorisnici { get; set; }
        public DbSet<VerifikacijskiKodovi> VerifikacijskiKodovi { get; set; }
        public DbSet<KorisnikFA> KorisnikFA { get; set; }
        public ApplicationDbContext(
            DbContextOptions options) : base(options)
        {
        }
      

    }
}
