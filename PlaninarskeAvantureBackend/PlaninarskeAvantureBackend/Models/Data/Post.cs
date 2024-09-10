using FIT_Api_Example.Data;
using FIT_Api_Example.Modul.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PlaninarskeAvantureBackend.Modul.Data
{
    public class Post
    {
        public int Id { get; set; }
        public string Tekst { get; set; }
        [ForeignKey(nameof(Korisnik))]
        public int KorisnikId { get; set; }
        public Korisnik Korisnik { get; set; }
        public string Tezina { get; set; }
        public string Vrijeme { get; set; }
        public int BrojLajkova { get; set; }
        public DateTime DatumKreiranja { get; set; }
        public List<Komentar>? Komentari { get; set; }
        public List<PostLajkovi>? Lajkovi { get; set; }

    }
}

