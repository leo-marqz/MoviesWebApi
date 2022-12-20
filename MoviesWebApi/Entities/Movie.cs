using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.Entities
{
    public class Movie : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InCinemas { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
        //--------------------------------
        public List<MoviesAuthors> MoviesAuthors { get; set; }
        public List<MoviesGenres> MoviesGenres { get; set; }
    }
}
