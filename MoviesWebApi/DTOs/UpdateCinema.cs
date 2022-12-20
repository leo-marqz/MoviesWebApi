using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class UpdateCinema
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
