namespace MoviesWebApi.DTOs
{
    public class TopMoviesList
    {
        public List<DisplayMovie> FutureReleases { get; set; }
        public List<DisplayMovie> InCinemas { get; set; }
    }
}
