using Microsoft.AspNetCore.Mvc;
using MoviesWebApi.Helpers;
using MoviesWebApi.Validations;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApi.DTOs
{
    public class CreateMovie
    {
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InCinemas { get; set; }
        public DateTime ReleaseDate { get; set; }
        [FileSizeValidation(maximunSizeMB: 4)]
        [FileTypeValidation(fileTypeGroup: FileTypeGroup.Image)]
        public IFormFile Poster { get; set; }
        //---------------------------------
        [ModelBinder(binderType: typeof(TypeBinder<List<int>>))]
        public List<int> GenresIds { get; set; }
        [ModelBinder(binderType: typeof(TypeBinder<List<CreateAuthorsMovie>>))]
        public List<CreateAuthorsMovie> Authors { get; set; }
    }
}
