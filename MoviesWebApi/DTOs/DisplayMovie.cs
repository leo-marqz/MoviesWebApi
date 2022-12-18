using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class DisplayMovie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool InCinemas { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Poster { get; set; }
    }
}
