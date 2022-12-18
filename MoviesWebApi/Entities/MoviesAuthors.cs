namespace MoviesWebApi.Entities
{
    public class MoviesAuthors
    {
        public int MovieId { get; set; }
        public int AuthorId { get; set; }
        public string Character { get; set; }
        public int Order { get; set; }
        public Movie Movie { get; set; }
        public Author Author { get; set; }
    }
}
