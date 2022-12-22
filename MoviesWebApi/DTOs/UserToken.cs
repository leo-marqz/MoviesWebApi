namespace MoviesWebApi.DTOs
{
    public class UserToken
    {
        public UserToken()
        {

        }
        public string Token { get; set; }
        public DateTime Expires { get; set; }

    }
}
