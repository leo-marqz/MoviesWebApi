namespace MoviesWebApi.DTOs
{
    public class MovieFilter
    {
        public int Page { get; set; } = 1;
        public int recordsPerPage { get; set; } = 10;
        public Pagination Pagination {
            get { return new Pagination { Page= Page, recordsPerPage = recordsPerPage }; }
        }
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool InCinemas { get; set; }
        public bool NextReleases { get; set; }
        public string FieldSort { get; set; }
        public bool AscendingOrder { get; set; }
    }
}
