using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class PatchAuthor
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
