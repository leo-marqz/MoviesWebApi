namespace MoviesWebApi.DTOs
{
    public class DetailsOfMovies : DisplayMovie
    {
        public List<DisplayGenre> Genres { get; set; }
        public List<DetailsOfAuthor> Authors { get; set; }
    }
}
