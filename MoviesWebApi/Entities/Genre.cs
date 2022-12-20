using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
    public class Genre : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string Name { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
