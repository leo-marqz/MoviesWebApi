using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class CreateGenre
    {
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
    }
}
