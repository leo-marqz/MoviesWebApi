using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class PatchMovie
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InCinemas { get; set; }
        public DateTime ReleaseDate { get; set; }
    }
}
