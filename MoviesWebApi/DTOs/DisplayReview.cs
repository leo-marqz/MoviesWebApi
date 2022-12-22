using Microsoft.AspNetCore.Identity;
using MoviesWebApi.Entities;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class DisplayReview
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Puntuation { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
