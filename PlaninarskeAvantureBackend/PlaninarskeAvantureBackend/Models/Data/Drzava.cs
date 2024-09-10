using System.ComponentModel.DataAnnotations;

namespace FIT_Api_Example.Modul.Data
{
    public class Drzava
    {
        [Key]
        public int Id { get; set; }
        public string Naziv { get; set; }
    }
}
