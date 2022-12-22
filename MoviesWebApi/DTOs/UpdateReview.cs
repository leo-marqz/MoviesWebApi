using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class UpdateReview
    {
        public string Comment { get; set; }
        [Range(1, 5)]
        public int Puntuation { get; set; }
    }
}
