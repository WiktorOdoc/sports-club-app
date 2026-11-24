using System.ComponentModel.DataAnnotations;

namespace SportsClubApp.Models
{
    public class Rider
    {
        public int Id { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }

        [Required(ErrorMessage = "Podaj imię zawodnika")]
        [StringLength(50)]
        [Display(Name = "Imię")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Podaj nazwisko zawodnika")]
        [StringLength(50)]
        [Display(Name = "Nazwisko")]
        public string LastName { get; set; } = string.Empty;

        [Range(1900, 2100, ErrorMessage = "Podaj prawidłowy rok urodzenia")]
        [Display(Name = "Rok urodzenia")]
        public int BirthYear { get; set; }

        [Range(0, 999, ErrorMessage = "Podaj prawidłowy numer zawodnika")]
        [Display(Name = "Numer startowy")]
        public int RacingNumber { get; set; }
    }
}
