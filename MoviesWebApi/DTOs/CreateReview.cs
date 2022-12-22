using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class CreateReview
    {
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Puntuation { get; set; }
    }
}
