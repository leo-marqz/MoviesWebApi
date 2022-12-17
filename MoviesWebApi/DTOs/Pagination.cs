namespace MoviesWebApi.DTOs
{
    public class Pagination
    {
        public int Page { get; set; } = 1;
        public int recordsPerPage = 10; //records per page
        public readonly int maximunRecordsPerPage = 50; // maximun records per page

        public int RecordsPerPage {
            get => recordsPerPage;
            set
            {
                recordsPerPage = (value > maximunRecordsPerPage) ? maximunRecordsPerPage : value;
            }
        }
    }
}
