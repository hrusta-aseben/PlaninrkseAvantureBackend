﻿namespace PlaninarskeAvantureBackend.ViewModels
{
    public class IzvjestajAddVM
    {
        public int DogadjajId { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public string Opis { get; set; }
        public string Ostecenja { get; set; }
        public string Povreda { get; set; }
        public IFormFile SlikaIzvjestaj{ get; set; }
    }
}
